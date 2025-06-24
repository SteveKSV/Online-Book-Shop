namespace Basket.DTO
{
    public class BasketItemDTO
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
