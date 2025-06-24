using Client.Models.Catalog.Ratings;
using Client.Services.Interfaces;

namespace Client.Services
{
    public class RatingService : IRatingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public RatingService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<int?> GetUserRatingAsync(Guid bookId, Guid userId)
        {
            var response = await _httpClient.GetAsync($"{_config["apiUrl"]}/rating/{bookId}/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<int>();
                return result;
            }
            return null;
        }

        public async Task<bool> RateBookAsync(RateBook entity)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_config["apiUrl"]}/rating", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRatingAsync(UpdateRating entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_config["apiUrl"]}/rating", entity);
            return response.IsSuccessStatusCode;
        }
    }
}
