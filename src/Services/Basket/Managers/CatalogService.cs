using Basket.DTO;
using Basket.Managers.Interfaces;

namespace Basket.Managers
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookDTO?> GetBookByIdAsync(Guid bookId)
        {
            var response = await _httpClient.GetAsync($"catalog/{bookId}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadFromJsonAsync<BookDTO>();
            return content;
        }
    }

}
