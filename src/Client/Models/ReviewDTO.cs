namespace Client.Models
{
    public class ReviewDTO
    {
        public string Id { get; set; }  
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public string BookId { get; set; }
        public bool ShowFullText { get; set; } = false;
    }
}
