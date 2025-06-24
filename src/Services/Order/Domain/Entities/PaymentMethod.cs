using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("PaymentMethods")]
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
