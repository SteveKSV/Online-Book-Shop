namespace Client.Models.Ordering
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }

        public Guid StatusId { get; set; }
        public string StatusName { get; set; }
        public PaymentDTO Payment { get; set; }
        public List<OrderItemDTO> Items { get; set; }

    }
    public class OrderItemDTO
    {
        public Guid BookId { get; set; }
        public BookDTO Book { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class BookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }

    public class PaymentDTO
    {
        public Guid Id { get; set; }
        public string Method { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
