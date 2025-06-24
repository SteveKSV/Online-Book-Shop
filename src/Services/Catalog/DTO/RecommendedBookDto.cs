namespace Catalog.DTO
{
    public class RecommendedBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string CoverImage { get; set; }
        public string Genre { get; set; }
        public double AverageRating { get; set; }
        public decimal Price { get; set; }
        public float RelevanceScore { get; set; }
    }
}
