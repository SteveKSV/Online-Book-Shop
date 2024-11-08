using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Catalog.Entities
{
    public class WarehouseBook
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("authors")]
        public string authors { get; set; }

        [BsonElement("image")]
        public string image { get; set; }

        [BsonElement("previewLink")]
        public string previewLink { get; set; }

        [BsonElement("publisher")]
        public string publisher { get; set; }

        [BsonElement("publishedDate")]
        public DateTime publishedDate { get; set; }

        [BsonElement("infoLink")]
        public string infoLink { get; set; }

        [BsonElement("ratingsCount")]
        public double ratingsCount { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }
    }
}
