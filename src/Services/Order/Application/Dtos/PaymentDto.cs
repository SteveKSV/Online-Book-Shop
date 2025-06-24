namespace Application.Dtos
{
    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public PaymentMethodDto PaymentMethod { get; set; }
        public CardPaymentDto? CardPayment { get; set; }
    }
}
