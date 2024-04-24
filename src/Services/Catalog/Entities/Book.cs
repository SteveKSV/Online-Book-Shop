using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Catalog.Entities
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Genre")]
        public string Genre { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("NumberOfPages")]
        public int NumberOfPages { get; set; }

        [BsonElement("PublicationDate")]
        public DateTime PublicationDate { get; set; }

        [BsonElement("LanguageName")]
        public string LanguageName { get; set; }

        [BsonElement("PublisherName")]
        public string PublisherName { get; set; }

        [BsonElement("AuthorName")]
        public string AuthorName { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }
    }
}
