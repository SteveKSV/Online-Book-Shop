namespace Catalog.DTO
{
    public class AddCommentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; }
    }
}
