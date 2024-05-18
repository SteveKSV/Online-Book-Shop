using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
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
            var anonymousCartId = _navigationManager.Uri.Contains("localhost")
                ? "anonymous_localhost"
                : Guid.NewGuid().ToString(); // Replace this with an actual unique ID generator if necessary

            if (anonymousCartId == null)
            {
                anonymousCartId = Guid.NewGuid().ToString();
                _navigationManager.NavigateTo("/", true); // Store the ID in a cookie or local storage
            }

            return anonymousCartId;
        }

        public async Task<ShoppingCart> GetCart()
        {
            await InitializeUserAsync();
            if (string.IsNullOrEmpty(UserName))
            {
                // Handle case where username claim is not found
                throw new InvalidOperationException("Username claim not found.");
            }
            
            var response = await _httpClient.GetAsync($"{_configuration.GetSection("apiUrl").Value}/basket/{UserName}");

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response body into a ShoppingCart object
                var cart = await response.Content.ReadFromJsonAsync<ShoppingCart>();
                return cart;
            }
            else
            {
                // Handle other error cases
                throw new HttpRequestException($"Error retrieving shopping cart. Status code: {response.StatusCode}");
            }
        }
        public string GetUserNameFromClaims(ClaimsPrincipal user)
        {
            var nameClaim = user.Identities
                                .SelectMany(identity => identity.Claims)
                                .FirstOrDefault(claim => claim.Type == "name");

            return nameClaim?.Value;
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

            var response = await _httpClient.PostAsJsonAsync($"{_configuration.GetSection("apiUrl").Value}/basket", cart);
            if (response.IsSuccessStatusCode)
            {
                CartChanged?.Invoke(this, new EventArgs());
            }
            else
            {
                // Handle other error cases
                throw new HttpRequestException($"Error retrieving shopping cart. Status code: {response.StatusCode}");
            }
        }
        public async Task<ShoppingCart> UpdateItemQuantity(string productId, int quantity)
        {
            var response = await _httpClient.PutAsync($"{_configuration.GetSection("apiUrl").Value}/basket/{UserName}/items/{productId}/quantity/{quantity}", null);
            if (response.IsSuccessStatusCode)
            {
                var cart = await response.Content.ReadFromJsonAsync<ShoppingCart>();
                CartChanged?.Invoke(this, new EventArgs());
                return cart;
            }
            else
            {
                throw new HttpRequestException($"Error updating item quantity. Status code: {response.StatusCode}");
            }
        }
        public async Task<ShoppingCart> RemoveFromCart(string productId)
        {
            var response = await _httpClient.DeleteAsync($"{_configuration.GetSection("apiUrl").Value}/basket/{UserName}/items/{productId}");
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response body into a ShoppingCart object
                var cart = await response.Content.ReadFromJsonAsync<ShoppingCart>();
                CartChanged?.Invoke(this, new EventArgs());
                return cart;
            }
            else
            {
                // Handle other error cases
                throw new HttpRequestException($"Error retrieving shopping cart. Status code: {response.StatusCode}");
            }
        }
    }
}
