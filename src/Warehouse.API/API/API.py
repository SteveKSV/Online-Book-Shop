from flask import Flask, jsonify, request
import joblib
from pymongo import MongoClient
from sklearn.metrics import accuracy_score, precision_score, recall_score, f1_score, confusion_matrix
from sklearn.utils.extmath import softmax
import numpy as np
import pandas as pd
import pyodbc
import uuid
from bson.binary import Binary, UUID_SUBTYPE
import os
import datetime
import csv

base_path = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(base_path, "training_logs.csv")
app = Flask(__name__)

# Load pre-trained components
label_encoder = joblib.load(os.path.join(base_path, 'labelEncoders_multiNB.pkl'))
model = joblib.load(os.path.join(base_path, 'model_multiNB.pkl'))
vectorizer = joblib.load(os.path.join(base_path, 'vectorizer_multiNB.pkl'))

try:
    genres_df = pd.read_csv(os.path.join(base_path, '../GenresTable.csv'))
    genre_mapping = {row['Name']: row['Id'] for _, row in genres_df.iterrows()}
except Exception as e:
    print(f"Failed to load genre mapping: {e}")
    

# MongoDB connection
client = MongoClient("mongodb://localhost:27017/")
db = client['Warehouse']
warehouse_collection = db["warehouse_data"]

sql_conn = pyodbc.connect(
    "DRIVER={ODBC Driver 17 for SQL Server};SERVER=STEPAN;DATABASE=WhatToRead;UID=sa;PWD=0x7s-gro9-2rdy"
)
sql_cursor = sql_conn.cursor()

def decode_uuid(binary_id):
    if isinstance(binary_id, Binary) and binary_id.subtype == UUID_SUBTYPE:
        return str(uuid.UUID(bytes=binary_id))
    return str(binary_id)

def predict_genres(description):
    description_vectorized = vectorizer.transform([description])
    probabilities = model.predict_proba(description_vectorized)[0]

    genre_probabilities = {
        label_encoder.inverse_transform([i])[0]: round(prob * 100, 2)
        for i, prob in enumerate(probabilities)
    }

    sorted_genres = dict(sorted(genre_probabilities.items(), key=lambda item: item[1], reverse=True))
    return sorted_genres

def entropy_sampling(model, books, n_samples):
    descriptions = [book['Description'] for book in books]
    description_vectorized = vectorizer.transform(descriptions)

    probs = model.predict_proba(description_vectorized)
    entropies = -np.sum(probs * np.log(probs + 1e-12), axis=1)

    # Вибираємо n_samples книг з найбільшою ентропією (найбільш невизначені)
    top_indices = np.argsort(entropies)[-n_samples:]
    selected_books = [books[i] for i in top_indices]

    # Додаємо ентропію до кожної книги (опційно, для UI чи аналізу)
    for i in range(len(selected_books)):
        selected_books[i]['Uncertainty'] = round(float(entropies[top_indices[i]]), 4)

    return selected_books

@app.route('/get_books_with_predictions', methods=['GET'])
def get_books_with_predictions():
    skip = int(request.args.get('skip', 0))
    limit = int(request.args.get('limit', 70))

    books = list(warehouse_collection.find({"genres": {"$exists": False}}).skip(skip).limit(limit * 2))

    if not books:
        return jsonify({"Books": []}), 200

    selected_books = entropy_sampling(model, books, n_samples=limit)

    books_with_predictions = []
    for book in selected_books:
        predictions = predict_genres(book['Description'])
        predictions_list = [{"Name": genre, "Probability": prob} for genre, prob in predictions.items()]

        books_with_predictions.append({
            "Id": decode_uuid(book['Id']),
            "Title": book['Title'],
            "Description": book['Description'],
            "Predictions": predictions_list,
            "Uncertainty": None
        })

    return jsonify({"Books": books_with_predictions}), 200

def retrain_model():
    try:
        labeled_books = list(warehouse_collection.find({"genres": {"$exists": True}}))
        
        if not labeled_books:
            print("No labeled books found for retraining.")
            return

        descriptions = [book['Description'] for book in labeled_books]
        genres = [book['genres'] for book in labeled_books]

        X_train = vectorizer.transform(descriptions)
        y_train = label_encoder.transform(genres)
        model.fit(X_train, y_train)
        print("Model retrained successfully.")
        
        # Оцінка на тих же даних (або тут можна підставити X_val, y_val, якщо є валідація)
        y_pred = model.predict(X_train)

        accuracy = accuracy_score(y_train, y_pred)
        precision = precision_score(y_train, y_pred, average='weighted', zero_division=0)
        recall = recall_score(y_train, y_pred, average='weighted', zero_division=0)
        f1 = f1_score(y_train, y_pred, average='weighted', zero_division=0)
        conf_matrix = confusion_matrix(y_train, y_pred).tolist()  # список списків

        # Підготувати рядок для CSV
        timestamp = datetime.datetime.utcnow().isoformat()
        csv_row = {
            "timestamp": timestamp,
            "accuracy": accuracy,
            "precision": precision,
            "recall": recall,
            "f1_score": f1,
            # Збережемо матрицю плутанини як JSON-подібний рядок
            "confusion_matrix": str(conf_matrix)
        }

        # Якщо CSV ще не існує – створюємо з заголовком
        file_exists = os.path.isfile(CSV_PATH)
        if not file_exists:
            with open(CSV_PATH, mode="w", encoding="utf-8", newline="") as csvfile:
                writer = csv.DictWriter(csvfile, fieldnames=csv_row.keys())
                writer.writeheader()
                writer.writerow(csv_row)
        else:
            with open(CSV_PATH, mode="a", encoding="utf-8", newline="") as csvfile:
                writer = csv.DictWriter(csvfile, fieldnames=csv_row.keys())
                writer.writerow(csv_row)

        print(f"Logged metrics to {CSV_PATH} at {timestamp}")
        
        for book in labeled_books:
            genre_name = book['genres']
            genre_id = genre_mapping.get(genre_name)

            if not genre_id:
                print(f"Genre ID not found for {genre_name}, skipping.")
                continue

            try:
                sql_cursor.execute("""
                    INSERT INTO Books (
                        Id, Title, Authors, GenreId, Price, StockQuantity,
                        Description, CoverImage, Publisher, PublishedAt, AverageRating
                    )
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """,
                    decode_uuid(book['Id']),
                    book["Title"],
                    book["Authors"],
                    genre_id,
                    book["Price"],
                    book.get("StockQuantity", 10),
                    book["Description"],
                    book.get("CoverImage", ""),
                    book.get("Publisher", ""),
                    datetime.datetime.now(),
                    float(book.get("AverageRating", 1))
                )
                sql_conn.commit()
                
                # Якщо commit пройшов — видаляємо з MongoDB
                try:
                    warehouse_collection.delete_one({
                        "Id": Binary(uuid.UUID(decode_uuid(book['Id'])).bytes, UUID_SUBTYPE)
                    })
                except Exception as delete_err:
                    print(f"Book inserted into SQL, but failed to delete from MongoDB: {delete_err}")

            except Exception as e:
                print(f"Error inserting into SQL Server for book {book['Title']}: {e}")

        print("Books successfully moved to SQL Server.")

    except Exception as e:
        print(f"An error occurred during retraining and moving books: {e}")

@app.route('/update_books_genres', methods=['POST'])
def update_books_genres():
    try:
        reviewed_books = request.json

        for book in reviewed_books:
            book_uuid = uuid.UUID(book['bookId'])
            book_id_binary = Binary(book_uuid.bytes, UUID_SUBTYPE)
            selected_genre = book['selectedGenre']

            result = warehouse_collection.update_one(
                {"Id": book_id_binary},
                {"$set": {"genres": selected_genre}}
            )

            if result.matched_count == 0:
                return jsonify({"error": f"Book with ID {book_id_binary} not found"}), 404

        if len(reviewed_books) >= 50:
            retrain_model()

        return jsonify({"message": "Genres updated successfully for reviewed books"}), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5010)
