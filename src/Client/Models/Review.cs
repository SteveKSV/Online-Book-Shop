namespace Client.Models
{
    public class Review
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Helpfulness { get; set; } = "0/0";
        public int Score { get; set; }
        public int ReviewTime { get; set; } = 0;
        public string Summary { get; set; }
        public string Text { get; set; }
        public string BookId { get; set; }
    }
}
