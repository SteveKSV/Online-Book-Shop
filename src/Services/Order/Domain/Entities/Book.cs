using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Books")]
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
