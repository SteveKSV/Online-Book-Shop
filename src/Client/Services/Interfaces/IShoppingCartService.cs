using Client.Models.Basket;
using System.Security.Claims;

namespace Client.Services.Interfaces
{
    public interface IShoppingCartService
    {
        event EventHandler CartChanged;
        Task<ShoppingCart> GetCart();
        Task<int> GetItemCountAsync();
        Task AddOrUpdateItem(Guid productId, int quantity, decimal price);
        Task<ShoppingCart> UpdateItemQuantity(Guid productId, int quantity);
        Task<ShoppingCart> RemoveFromCart(Guid productId);
    }
}
