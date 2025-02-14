using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Catalog.Entities
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("UserId")]
        public Guid UserId { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("Helpfulness")]
        public string Helpfulness { get; set; }

        [BsonElement("Score")]
        public int Score { get; set; }

        [BsonElement("Time")]
        public int ReviewTime { get; set; } 

        [BsonElement("Summary")]
        public string Summary { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("BookId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }
    }
}
