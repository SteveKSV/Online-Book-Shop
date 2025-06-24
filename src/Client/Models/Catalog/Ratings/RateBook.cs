namespace Client.Models.Catalog.Ratings
{
    public class RateBook
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public int Rating { get; set; } = 1;
    }
}
