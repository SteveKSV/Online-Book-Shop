using Basket.Entities;

namespace Basket.Managers.Interfaces
{
    public interface IBasketManager
    {

        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task<ShoppingCart> UpdateItemQuantity(string userName, string productId, int quantity);
        Task DeleteBasket(string userName);
        Task<ShoppingCart> RemoveItemFromBasket(string userName, string productId);
    }
}
