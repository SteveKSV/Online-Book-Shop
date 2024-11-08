namespace Client.Models.ModelsForPrediction
{
    public class BookResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<GenrePrediction> Predictions { get; set; }
        public double Uncertainty { get; set; }

    }

}
