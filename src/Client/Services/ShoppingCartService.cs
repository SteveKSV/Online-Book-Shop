using Client.Models.Basket;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NuGet.ContentModel;
using System.Security.Claims;

namespace Client.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IConfiguration _configuration;

        public event EventHandler? CartChanged;

        public Guid UserId { get; private set; }

        public ShoppingCartService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            NavigationManager navigationManager,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
            _configuration = configuration;
        }

        private async Task InitializeUserAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                // Пріоритет: ClaimTypes.NameIdentifier -> "sub" -> "nameid"
                string? rawUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? user.FindFirst("sub")?.Value
                                    ?? user.FindFirst("nameid")?.Value;

                if (rawUserId != null && Guid.TryParse(rawUserId, out var parsedUserId))
                {
                    UserId = parsedUserId;
                }
                else
                {
                    throw new InvalidOperationException("UserId claim is missing or invalid.");
                }
            }
        }

        public async Task<ShoppingCart> GetCart()
        {
            await InitializeUserAsync();
            var response = await _httpClient.GetAsync($"{_configuration["apiUrl"]}/basket/{UserId}");
           
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ShoppingCart { UserId = UserId };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new ShoppingCart();
            }
            
            var basket = await response.Content.ReadFromJsonAsync<ShoppingCart>();
            return basket!;
        }

        public async Task<int> GetItemCountAsync()
        {
            await InitializeUserAsync();

            var response = await _httpClient.GetAsync($"{_configuration["apiUrl"]}/basket/{UserId}/count");

            if (!response.IsSuccessStatusCode)
            {
                return 0;
            }

            var count = await response.Content.ReadFromJsonAsync<int>();
            return count;
        }

        public async Task<ShoppingCart> UpdateItemQuantity(Guid productId, int quantity)
        {
            await InitializeUserAsync();

            var response = await _httpClient.PutAsync(
                $"{_configuration["apiUrl"]}/basket/{UserId}/items/{productId}/quantity/{quantity}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to update quantity.");
            }

            var item = await response.Content.ReadFromJsonAsync<ShoppingCartItem>();
            return await GetCart();
        }

        public async Task<ShoppingCart> RemoveFromCart(Guid productId)
        {
            await InitializeUserAsync();

            var response = await _httpClient.DeleteAsync($"{_configuration["apiUrl"]}/basket/{UserId}/items/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to remove item.");
            }

            return await GetCart();
        }

        public async Task AddOrUpdateItem(Guid productId, int quantity, decimal price)
        {
            await InitializeUserAsync();

            var item = new BasketItemDTO
            {
                BookId = productId,
                Quantity = quantity,
                Price = price
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_configuration["apiUrl"]}/basket/{UserId}/items",
                item);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to add/update item.");
            }

            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task ClearCart()
        {
            await InitializeUserAsync();

            var response = await _httpClient.DeleteAsync($"{_configuration["apiUrl"]}/basket/{UserId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to clear basket.");
            }

            CartChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
