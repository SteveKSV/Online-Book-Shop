namespace Catalog.Entities
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int Value { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
