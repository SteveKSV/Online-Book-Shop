using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Payments")]
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public decimal Amount { get; set; }

        public Guid PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ICollection<CardPayment> CardPayments { get; set; }
    }
}
