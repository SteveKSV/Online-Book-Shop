namespace EventBusMessages.Common
{
    public class Order
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public int PaymentMethod { get; set; }
        public List<OrderItem> Items { get; set; }
    }

}
