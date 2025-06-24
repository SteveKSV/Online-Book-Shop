using Catalog.Entities;

namespace Catalog.DTO
{
    public class RatingDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public float Value { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
