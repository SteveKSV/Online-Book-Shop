namespace Catalog.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public ICollection<OrderItem?> Items { get; set; }
    }
}
