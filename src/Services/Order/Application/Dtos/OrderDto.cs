using Domain.Entities;

namespace Application.Dtos
{
    public class OrderDto
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
        public string StatusName { get; set; }
        public PaymentDto Payment { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public Guid BookId { get; set; }
        public BookDto Book { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
