namespace Catalog.DTO
{
    public class UpdateRatingDTO
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public int NewRating { get; set; }
    }

}
