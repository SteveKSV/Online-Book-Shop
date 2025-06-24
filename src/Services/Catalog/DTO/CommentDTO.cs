namespace Catalog.DTO
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CommentText { get; set; }
        public string Username { get; set; }
        public DateTime CommentedAt { get; set; }
    }
}
