namespace Domain.Entities
{
    public class OrderStatus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
