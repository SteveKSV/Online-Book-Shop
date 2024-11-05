using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Client.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        public string UserName { get; private set; }
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IConfiguration _configuration;

        public event EventHandler CartChanged;

        public ShoppingCartService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _navigationManager = navigationManager;
            _configuration = configuration;
        }

        private async Task InitializeUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            UserName = authState.User.Identity.IsAuthenticated
                        ? GetUserNameFromClaims(authState.User)
                        : GetAnonymousCartId();
        }

        private string GetAnonymousCartId()
        {
            return _navigationManager.Uri.Contains("localhost") ? "anonymous_localhost" : Guid.NewGuid().ToString();
        }

        public async Task<ShoppingCart> GetCart()
        {
            await InitializeUserAsync();
            if (string.IsNullOrEmpty(UserName))
            {
                throw new InvalidOperationException("Username claim not found.");
            }

            var response = await _httpClient.GetAsync($"{_configuration["apiUrl"]}/basket/{UserName}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ShoppingCart>() ?? new ShoppingCart();
            }
            else
            {
                throw new HttpRequestException($"Error retrieving shopping cart. Status code: {response.StatusCode}");
            }
        }

        public string GetUserNameFromClaims(ClaimsPrincipal user)
        {
            var nameClaim = user.FindFirst(ClaimTypes.Name);
            return nameClaim?.Value ?? throw new InvalidOperationException("Username claim not found.");
        }

        public async Task<int> GetItemCountAsync()
        {
            var cart = await GetCart();
            return cart.Items.Count;
        }

        public async Task AddToCart(ShoppingCartItem item)
        {
            var cart = await GetCart();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            var response = await _httpClient.PostAsJsonAsync($"{_configuration["apiUrl"]}/basket", cart);
            if (response.IsSuccessStatusCode)
            {
                CartChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                throw new HttpRequestException($"Error updating cart. Status code: {response.StatusCode}");
            }
        }

        public async Task<ShoppingCart> UpdateItemQuantity(string productId, int quantity)
        {
            await InitializeUserAsync();
            var response = await _httpClient.PutAsync($"{_configuration["apiUrl"]}/basket/{UserName}/items/{productId}/quantity/{quantity}", null);

            if (response.IsSuccessStatusCode)
            {
                CartChanged?.Invoke(this, EventArgs.Empty);
                return await response.Content.ReadFromJsonAsync<ShoppingCart>() ?? new ShoppingCart();
            }
            else
            {
                throw new HttpRequestException($"Error updating item quantity. Status code: {response.StatusCode}");
            }
        }

        public async Task<ShoppingCart> RemoveFromCart(string productId)
        {
            await InitializeUserAsync();
            var response = await _httpClient.DeleteAsync($"{_configuration["apiUrl"]}/basket/{UserName}/items/{productId}");

            if (response.IsSuccessStatusCode)
            {
                CartChanged?.Invoke(this, EventArgs.Empty);
                return await response.Content.ReadFromJsonAsync<ShoppingCart>() ?? new ShoppingCart();
            }
            else
            {
                throw new HttpRequestException($"Error removing item from cart. Status code: {response.StatusCode}");
            }
        }
    }
}
