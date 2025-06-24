using Client.Models.Ordering;
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

        public async Task<List<OrderDTO>> GetAllOrdersByUserId(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_configuration["apiUrl"]}/order/GetByUserId/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<OrderDTO>>();
                    return data ?? new List<OrderDTO>();
                }
                else
                {
                    throw new HttpRequestException($"Error fetching orders. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching user orders.", ex);
            }
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

        public async Task<List<PaymentMethod>> GetPaymentMethods()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_configuration["apiUrl"]}/paymentMethod");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<PaymentMethod>>();
                    return data ?? new List<PaymentMethod>();
                }
                else
                {
                    throw new HttpRequestException($"Error fetching payment methods. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Тут можна залогувати або обробити помилку по-іншому
                throw new ApplicationException("An error occurred while fetching payment methods.", ex);
            }
        }

    }
}
