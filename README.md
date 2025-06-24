# 📚 Intelligent WhatToRead BookStore Platform

A microservices-based web platform for selling and managing books with integrated machine learning for automatic genre classification. Developed as part of a final degree project in Computer Science.

---

## ✨ Features

- 🔐 User registration and authentication (JWT)
- ⚙️ Account management (edit personal info)
- 📖 Book catalog with:
  - Pagination
  - Filtering by genre
  - Sorting
  - Full book details
- 🛒 Shopping cart and order placement
- 🧠 Genre classification using active machine learning where admin can label manually books if they want
- 🤖 Book recommendations based on classified genres

---

## 🧠 Machine Learning Integration

The application uses **NLP** and **Active Learning** to automatically classify books into genres based on their descriptions, reducing manual labor and improving recommendation quality.

### ML Features:
- Text preprocessing: stopword removal, lemmatization
- Feature extraction: CountVectorizer, TF-IDF
- Supervised learning: multiple models tested
- Active learning: entropy-based selection strategy
- Metrics logging and visualization
- Automatic retraining after labeling

---

## 📊 ML Model Benchmark

| Model              | Accuracy | Precision | Recall | F1-score | Training Time |
|--------------------|----------|-----------|--------|----------|----------------|
| **MultinomialNB**  | **71.29%** | 71.90%    | 71.29% | 71.31%   | 0.11 sec       |
| Logistic Regression| 67.88%   | 68.64%    | 67.88% | 68.19%   | 29 sec         |
| MLP Classifier     | 67.11%   | 67.66%    | 67.11% | 67.30%   | 33 min         |
| Dense NN           | 66.63%   | 66.69%    | 66.63% | 65.57%   | 1 min          |
| Linear SVM         | 65.25%   | 66.16%    | 65.25% | 65.60%   | 3 min          |
| Random Forest      | 63.03%   | 63.32%    | 63.03% | 62.89%   | 14 min         |
| LSTM               | 61.54%   | 60.81%    | 61.54% | 60.49%   | 15 min         |
| GRU                | 60.24%   | 55.80%    | 60.24% | 56.54%   | 5 min          |
| BiLSTM             | 57.46%   | 50.52%    | 57.46% | 52.28%   | 15 min         |

> ✅ Best base model: **Multinomial Naive Bayes**  
> 📈 Accuracy after 40 active learning cycles: **72.93%**

---

## 🔁 Active Learning Experiment

| Model         | Data (2000 books)   | Accuracy | Recall | Precision | F1-score |
|---------------|---------------------|----------|--------|-----------|----------|
| MultinomialNB | 40 iterations × 50  | 72.93%   | 72.93% | 73.23%    | 72.89%   |

---

## 🧱 Architecture & Stack

### 🧩 Architecture
- Microservices with REST API
- API Gateway pattern
- CQRS pattern
- Message Broker
- Generic Repository pattern
- Domain-Driven Design (DDD)

### 🧩 System Architecture
![image](https://github.com/user-attachments/assets/a35c1585-09e0-44d0-ad41-4fb86910e987)

### ⚙️ Technologies

| Layer       | Tools/Frameworks                                  |
|-------------|----------------------------------------------------|
| Backend     | ASP.NET Core Web API, MongoDB, SQL Server, RabbitMQ|
| Frontend    | Blazor Server                                      |
| ML / NLP    | Python (scikit-learn, NLTK, Keras, TensorFlow)     |
| DevOps      | Docker-ready services, CSV logging, monitoring     |

---

## 🗂 Project Structure

📦 WhatToReadApp
├── Client # Blazor frontend
├── Gateway # API Gateway
├── Identity.API # Auth microservice
├── Catalog.API # Book management
├── Basket.API # Shopping cart
├── Order.API # Orders and checkout
├── Flask.API # Active learning pipeline (Python)
└── Shared # Common resources


---

## 📝 Project Outcomes

- Business domain analysis and competitive research
- Product Vision Board, Roadmap, User Story Map
- End-to-end system design and implementation
- Comparison of ML models for genre classification
- Deployment-ready microservice system
- ML pipeline integrated into real data flow

---

## 📌 Future Improvements

- Collaborative filtering recommender system
- Enhanced genre hierarchy and multi-label classification
- UI/UX refinement for mobile view
- Docker Compose orchestration for full stack

---
## 📎 License

This project is licensed under the [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/) – for non-commercial and educational use only.
