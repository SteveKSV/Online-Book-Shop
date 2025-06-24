namespace Client.Models.Basket
{
    public class ShoppingCart
    {
        public Guid UserId { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
