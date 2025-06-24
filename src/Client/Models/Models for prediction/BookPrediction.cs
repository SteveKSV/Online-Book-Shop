namespace Client.Models.ModelsForPrediction
{
    public class BookPrediction
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Prediction> Predictions { get; set; }
        public double? Uncertainty { get; set; }
    }

}
