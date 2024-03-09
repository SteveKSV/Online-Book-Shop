using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;
        public CatalogService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await _client.GetAsync("/api/Book/");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _client.GetAsync($"/api/Book/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }
        public async Task<IEnumerable<CatalogModel>> GetCatalogByGenre(string genre)
        {
            var response = await _client.GetAsync($"/api/Book/GetBooksByGenre/{genre}");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
    }
}
