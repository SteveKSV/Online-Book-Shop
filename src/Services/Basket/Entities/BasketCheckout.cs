namespace Basket.Entities
{
    public class BasketCheckout
    {
        public Guid UserId { get; set; }

        // Billing
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }

        public string? CardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }

        // Payment
        public Guid PaymentMethodId { get; set; }


    }
}
