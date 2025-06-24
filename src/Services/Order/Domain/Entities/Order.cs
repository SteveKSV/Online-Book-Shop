using Domain.Common;

namespace Domain.Entities
{
    public class Order : EntityBase
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
       
        public Guid StatusId { get; set; }           
        public OrderStatus Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment Payment { get; set; }
    }

}
