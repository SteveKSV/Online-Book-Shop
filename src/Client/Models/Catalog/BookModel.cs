namespace Client.Models.Catalog
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public Guid GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoverImage { get; set; } = "https://www.forewordreviews.com/books/covers/the-official-librarian.jpg";
        public string Publisher { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; } = DateTime.Now;
        public float AverageRating { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }


}
