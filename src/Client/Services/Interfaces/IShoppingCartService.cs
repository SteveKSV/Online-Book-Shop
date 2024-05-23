using Client.Models;
using System.Security.Claims;

namespace Client.Services.Interfaces
{
    public interface IShoppingCartService
    {
        event EventHandler CartChanged;
        string GetUserNameFromClaims(ClaimsPrincipal user);
        Task<int> GetItemCountAsync();
        Task<ShoppingCart> GetCart();
        Task AddToCart(ShoppingCartItem item);
        Task<ShoppingCart> UpdateItemQuantity(string productId, int quantity);
        Task<ShoppingCart> RemoveFromCart(string productId);
    }
}
