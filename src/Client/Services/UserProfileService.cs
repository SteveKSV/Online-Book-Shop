using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserProfileService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task ChangeUser(UpdateUser updateUser)
        {
            var identityUrl = $"{_configuration.GetSection("IdentityServerSettings")["DiscoveryUrl"]}/Account/ChangeUser/{updateUser.OldUsername}";
            var basketUrl = $"{_configuration.GetSection("apiUrl").Value}/basket/{updateUser.OldUsername}/update-username/{updateUser.NewUsername}";
            var ordersUrl = $"{_configuration.GetSection("apiUrl").Value}/order/update-username/{updateUser.OldUsername}/{updateUser.NewUsername}";

            // Оновити ім'я користувача в Identity Server
            var resultIdentity = await _httpClient.PutAsJsonAsync(identityUrl, updateUser);

            if (!resultIdentity.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update username in Identity Server.");
            }

            if (updateUser.NewUsername != null)
            {
                // Оновити ім'я користувача в системі кошика
                var resultBasket = await _httpClient.PutAsJsonAsync(basketUrl, updateUser);
                if (!resultBasket.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to update username in Basket system.");
                }

                // Оновити ім'я користувача в системі замовлень
                var resultOrders = await _httpClient.PutAsJsonAsync(ordersUrl, updateUser);
                if (!resultOrders.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to update username in Orders system.");
                }
            }
        }

        public async Task<string> GetUserEmail(string username)
        {
            var result = await _httpClient.GetAsync($"{_configuration.GetSection("IdentityServerSettings")["DiscoveryUrl"]}/Account/Email/{username}");
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
    }
}
