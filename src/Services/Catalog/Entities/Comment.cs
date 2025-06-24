using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Entities
{
    [Table("Comments")]
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentedAt { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
    }
}
