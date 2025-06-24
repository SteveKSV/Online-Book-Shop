namespace Client.Models.Catalog.Ratings
{
    public class UpdateRating
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public int NewRating { get; set; } = 1;
    }
}
