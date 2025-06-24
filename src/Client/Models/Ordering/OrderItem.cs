namespace Client.Models.Ordering
{
    public class OrderItem
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
