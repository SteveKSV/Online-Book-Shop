namespace Shopping.Aggregator.Models
{
    public class CatalogModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public int NumberOfPages { get; set; }
        public DateTime PublicationDate { get; set; }
        public string LanguageName { get; set; }
        public string PublisherName { get; set; }
        public string AuthorName { get; set; }
    }
}
