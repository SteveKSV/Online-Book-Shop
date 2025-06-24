namespace Client.Models.ModelsForPrediction
{
    public class ReviewedBook
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string SelectedGenre { get; set; }
    }

}
