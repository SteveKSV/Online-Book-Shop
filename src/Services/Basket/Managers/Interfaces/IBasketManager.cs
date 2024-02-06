using Basket.Entities;

namespace Basket.Managers.Interfaces
{
    public interface IBasketManager
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task DeleteBasket(string userName);
    }
}
