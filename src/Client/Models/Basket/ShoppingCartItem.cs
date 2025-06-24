namespace Client.Models.Basket
{
    public class ShoppingCartItem
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
