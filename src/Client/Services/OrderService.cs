using Client.Models;
using Client.Pages;
using Client.Services.Interfaces;

namespace Client.Services
{
    public class OrderService : IOrderService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task PlaceOrder(BasketCheckout order)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_configuration.GetSection("apiUrl").Value}/basket/checkout", order);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                // Handle other error cases
                throw new HttpRequestException($"Error placing order. Status code: {response.StatusCode}");
            }
        }
    }
}
