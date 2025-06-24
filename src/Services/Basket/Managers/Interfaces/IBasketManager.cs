using Basket.DTO;
using Basket.Entities;

namespace Basket.Managers.Interfaces
{
    public interface IBasketManager
    {
        Task<BasketDTO> GetBasket(Guid userId);
        Task<BasketItem> AddOrUpdateItem(Guid userId, Guid bookId, int quantity, decimal price);
        Task<BasketItem> UpdateItemQuantity(Guid userId, Guid bookId, int quantity);
        Task DeleteBasket(Guid userId);
        Task<BasketDTO> RemoveItemFromBasket(Guid userId, Guid bookId);
    }
}
