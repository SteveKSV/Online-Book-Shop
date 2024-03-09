namespace Shopping.Aggregator.Models
{
    public class BasketItemExtendedModel
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        //Product Related Additional Fields

        public string Description { get; set; }
        public string Genre { get; set; }
        public int NumberOfPages { get; set; }
        public DateTime PublicationDate { get; set; }
        public string LanguageName { get; set; }
        public string PublisherName { get; set; }
        public string AuthorName { get; set; }
    }
}
