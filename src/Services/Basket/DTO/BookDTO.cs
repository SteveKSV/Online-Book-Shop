namespace Basket.DTO
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
    }
}
