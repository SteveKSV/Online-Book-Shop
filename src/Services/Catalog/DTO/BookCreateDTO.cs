namespace Catalog.DTO
{
    public class BookCreateDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public Guid GenreId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public string Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
