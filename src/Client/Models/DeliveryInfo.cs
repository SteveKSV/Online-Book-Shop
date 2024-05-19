using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class DeliveryInfo
    {
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        // BillingAddress
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Address Line is required")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        public string State { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip Code")]
        public string ZipCode { get; set; }

        // Payment
        [Required(ErrorMessage = "Card Name is required")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid Card Number")]
        public string CardName { get; set; }

        [Required(ErrorMessage = "Card Number is required")]
        [CreditCard(ErrorMessage = "Invalid Card Number")]
        public string CardNumber { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Invalid Expiration Date (MM/YY or MM/YYYY)")]
        [Required(ErrorMessage = "Expiration Date is required")]
        public string Expiration { get; set; }

        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV")]
        [Required(ErrorMessage = "CVV is required")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Payment Method is required")]
        public int PaymentMethod { get; set; }
    }
}
