using System.ComponentModel.DataAnnotations.Schema;

namespace Basket.Entities
{
    [Table("Basket")]
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
