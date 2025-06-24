using EventBusMessages.Common;

namespace EventBusMessages.Events
{
    public class BasketCheckoutEvent
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        // Billing
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }

        // Payment
        public Guid PaymentMethodId { get; set; }
        public string? CardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }
        // Items
        public List<OrderItem> Items { get; set; }
    }

}
