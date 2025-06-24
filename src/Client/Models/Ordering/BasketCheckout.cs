using System.ComponentModel.DataAnnotations;

namespace Client.Models.Ordering
{
    public class BasketCheckout
    {
        public Guid UserId { get; set; }

        // Billing
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Address { get; set; }

        // Payment
        [Required]
        public Guid PaymentMethodId { get; set; }

        public string? CardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }

    }
}
