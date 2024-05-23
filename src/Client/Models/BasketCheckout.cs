using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class BasketCheckout
    {
        public string UserName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // BillingAddress
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address. Format: (yourText)@(mail).com. Example: whatToRead@gmail.com")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Address line is required")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [RegularExpression(@"^\d{5}(?:-\d{4})?$", ErrorMessage = "Invalid zip code format. Use format XXXXX or XXXXX-XXXX")]
        public string ZipCode { get; set; }

        // Payment

        [Required(ErrorMessage = "Card name is required")]
        public string CardName { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid card number format. Must be 16 digits.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Invalid expiration date format (MM/YY)")]
        public string Expiration { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV format. Must be 3 or 4 digits.")]
        public string CVV { get; set; }
        public int PaymentMethod { get; set; }
    }
}
