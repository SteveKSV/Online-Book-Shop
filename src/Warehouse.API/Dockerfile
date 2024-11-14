FROM python:3.9-slim

# Set working directory
WORKDIR /app

# Copy requirements.txt first to install dependencies
COPY requirements.txt .

# Install dependencies
RUN pip install --no-cache-dir -r requirements.txt

# Copy the rest of the application files
COPY . .

# Expose the port the app runs on
EXPOSE 80

# Run the Flask app
CMD ["python", "API.py"]
