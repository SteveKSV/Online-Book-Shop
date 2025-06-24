namespace Application.Dtos
{
    public class UpdateOrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        // Billing Address
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }

        public Guid StatusId { get; set; }

        public PaymentDto Payment { get; set; }

        public ICollection<OrderItemDto> Items { get; set; }
    }
}
