namespace Catalog.DTO
{
    public class RateBookDTO
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public int Rating { get; set; }
    }
}
