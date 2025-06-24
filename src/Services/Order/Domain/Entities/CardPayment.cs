using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("CardPayments")]
    public class CardPayment
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Cvv { get; set; }
    }
}
