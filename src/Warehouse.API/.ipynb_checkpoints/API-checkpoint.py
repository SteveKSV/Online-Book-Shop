from flask import Flask, jsonify, request
import joblib
from pymongo import MongoClient
from bson import ObjectId
import numpy as np
from scipy.stats import entropy
import pandas as pd
import pyodbc

app = Flask(__name__)

# Load pre-trained components
label_encoder = joblib.load('ForModel/label_encoderMulti.pkl')
model = joblib.load('ForModel/modelMulti.pkl')
vectorizer = joblib.load('ForModel/vectorizerMulti.pkl')

genre_mapping = {}
try:
    genres_df = pd.read_csv('GenresTables.csv')
    # Очікується, що CSV має колонки 'Id' (GUID як str) та 'Name'
    genre_mapping = {row['Name']: row['Id'] for _, row in genres_df.iterrows()}
except Exception as e:
    print(f"Failed to load genre mapping: {e}")
    

# MongoDB connection
client = MongoClient("mongodb://mongo:27017/")
db = client['CatalogDb']
warehouse_collection = db["warehouseBooks"]
catalog_collection = db["book"]

sql_conn = pyodbc.connect(
    "DRIVER={ODBC Driver 17 for SQL Server};SERVER=STEPAN;DATABASE=WhatToRead;UID=sa;PWD=0x7s-gro9-2rdy"
)
sql_cursor = sql_conn.cursor()

# Prediction function using the loaded model
def predict_genres(description):
    description_vectorized = vectorizer.transform([description])
    probabilities = model.predict_proba(description_vectorized)[0]
    
    genre_probabilities = {
        label_encoder.inverse_transform([i])[0]: round(prob * 100, 2)  # Keep as numeric percentage, rounded to 2 decimals
        for i, prob in enumerate(probabilities)
    }
    
    # Sort the genres by their probabilities
    sorted_genres = dict(sorted(genre_probabilities.items(), key=lambda item: item[1], reverse=True))
    
    return sorted_genres  # Return as a dictionary

def uncertainty_sampling(model, books, n_samples):
    # Векторизація описів книг
    descriptions = [book['description'] for book in books]
    description_vectorized = vectorizer.transform(descriptions)
    
    # Отримуємо ймовірності для кожної книги
    probs = model.predict_proba(description_vectorized)
    
    # Обчислюємо невизначеність на основі ентропії
    uncertainty_values = []
    
    for prob in probs:
        # Обчислюємо ентропію як міру невизначеності
        entropy_value = entropy(prob)
        max_entropy = np.log(len(prob))  # Максимальна ентропія для кількості класів

        # Нормалізація ентропії для діапазону [0%, 100%]
        uncertainty = round((entropy_value / max_entropy) * 100, 2)

        uncertainty_values.append(uncertainty)
    
    # Сортуємо книги за невизначеністю (найвища невизначеність буде в кінці списку)
    uncertain_indices = np.argsort(uncertainty_values)[-n_samples:]  # Більше невизначених книг будуть в кінці
    
    # Повертаємо книги з найбільшою невизначеністю
    uncertain_books = [books[i] for i in uncertain_indices]
    
    return uncertain_books


@app.route('/get_books_with_predictions', methods=['GET'])
def get_books_with_predictions():
    skip = int(request.args.get('skip', 0))  # Default to 0 if no 'skip' is provided
    limit = int(request.args.get('limit', 70))  # Default to 70 books

    # Fetch the next set of books from MongoDB
   books = list(warehouse_collection.find({"genres": {"$exists": False}}).skip(skip).limit(limit))

    # Get the most uncertain books
    uncertain_books = uncertainty_sampling(model, books, n_samples=limit)

    # Create a list of books with predictions and uncertainty
    books_with_predictions = []
    for book in uncertain_books:
        predictions = predict_genres(book['description'])
        
        predictions_list = [{"Name": genre, "Probability": prob} for genre, prob in predictions.items()]
        
        # Отримуємо ентропію як міру невизначеності
        entropy_value = entropy(list(predictions.values()))
        max_entropy = np.log(len(predictions))  # Максимальна ентропія для кількості класів

        # Нормалізація ентропії для діапазону [0%, 100%]
        uncertainty = round((entropy_value / max_entropy) * 100, 2)
        
        books_with_predictions.append({
            "Id": str(book['_id']),
            "Title": book['Title'],
            "Description": book['description'],
            "Predictions": predictions_list,
            "Uncertainty": uncertainty  # У відсотках
        })

    # Sort the books based on uncertainty (highest uncertainty first)
    books_with_predictions_sorted = sorted(
        books_with_predictions, 
        key=lambda x: x['Uncertainty'], 
        reverse=True  # Highest uncertainty will be first
    )

    return jsonify({"Books": books_with_predictions_sorted}), 200

def retrain_model():
    try:
        labeled_books = list(warehouse_collection.find({"genres": {"$exists": True}}))
        
        if not labeled_books:
            print("No labeled books found for retraining.")
            return

        descriptions = [book['description'] for book in labeled_books]
        genres = [book['genres'] for book in labeled_books]

        X_train = vectorizer.transform(descriptions)
        y_train = label_encoder.transform(genres)
        model.fit(X_train, y_train)
        print("Model retrained successfully.")

        for book in labeled_books:
            genre_name = book['genres']
            genre_id = genre_mapping.get(genre_name)

            if not genre_id:
                print(f"Genre ID not found for {genre_name}, skipping.")
                continue

            try:
                sql_cursor.execute("""
                    INSERT INTO Books (Title, Authors, GenreId, Price, StockQuantity, Description, CoverImage, Publisher, PublishedAt, AverageRating)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, GETDATE(), ?)
                """, book["Title"], book["authors"], genre_id, book["Price"], book.get("StockQuantity", 10),
                     book["description"], book.get("image", ""), book.get("publisher", ""), float(book.get("ratingsCount", 1)))
                sql_conn.commit()
                
                # Успішно вставлено — видаляємо з Mongo
                warehouse_collection.delete_one({"_id": book["_id"]})

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
            book_id = book['bookId']
            selected_genre = book['selectedGenre']

            result = warehouse_collection.update_one(
                {"_id": ObjectId(book_id)},
                {"$set": {"genres": selected_genre}}
            )

            if result.matched_count == 0:
                return jsonify({"error": f"Book with ID {book_id} not found"}), 404

        if len(reviewed_books) >= 50:
            retrain_model()

        return jsonify({"message": "Genres updated successfully for reviewed books"}), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
