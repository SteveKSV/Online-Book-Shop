using Basket.Entities;

namespace Basket.DTO
{
    public class BasketDTO
    {
        public List<BasketItemDetailedDTO> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
