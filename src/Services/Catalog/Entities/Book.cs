using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Entities
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Authors { get; set; }
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublishedAt { get; set; } = DateTime.Now;
        public float AverageRating { get; set; } = 1;
        public ICollection<Comment> Comments { get; set; }
    }

}
