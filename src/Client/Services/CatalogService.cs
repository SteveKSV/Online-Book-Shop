using Client.Models.Catalog;
using Client.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Client.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CatalogService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(List<BookModel>, PaginationMetadata)> GetBooks(string? queryString = null)
        {
            // Перевірка та нормалізація query string
            string query = string.IsNullOrWhiteSpace(queryString) ? "" :
                           queryString.StartsWith("?") ? queryString : "?" + queryString;

            string url = $"{_configuration["apiUrl"]}/catalog{query}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return (null, null);

            var books = await response.Content.ReadFromJsonAsync<List<BookModel>>();
            var paginationMetadata = ParsePaginationMetadata(response.Headers);

            return (books ?? new List<BookModel>(), paginationMetadata);
        }

        public async Task<List<string>> Get(string queryString = null)
        {
            var url = $"{_configuration["apiUrl"]}/{queryString}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null!;

            return await response.Content.ReadFromJsonAsync<List<string>>();
        }

        public async Task<bool> AddCommentAsync(AddUpdateComment comment)
        {
            var url = $"{_configuration["apiUrl"]}/comment";
            var response = await _httpClient.PostAsJsonAsync(url, comment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCommentAsync(AddUpdateComment comment)
        {
            var url = $"{_configuration["apiUrl"]}/comment/update-comment";
            var response = await _httpClient.PutAsJsonAsync(url, comment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var url = $"{_configuration["apiUrl"]}/comment/{commentId}";
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        public async Task<(List<Comment>, PaginationMetadata)> GetCommentsForBook(Guid bookId, string? queryString = null)
        {
            // Перевірка та нормалізація query string
            string query = string.IsNullOrWhiteSpace(queryString) ? "" :
                           queryString.StartsWith("?") ? queryString : "?" + queryString;

            var url = $"{_configuration["apiUrl"]}/catalog/{bookId}/comments{query}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return (new List<Comment>(), new PaginationMetadata());

            var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();
            var paginationMetadata = ParsePaginationMetadata(response.Headers);

            return (comments ?? new List<Comment>(), paginationMetadata);
        }

        private PaginationMetadata ParsePaginationMetadata(HttpResponseHeaders headers)
        {
            if (headers.TryGetValues("X-Pagination", out var values))
            {
                var paginationHeader = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(paginationHeader))
                {
                    return JsonConvert.DeserializeObject<PaginationMetadata>(paginationHeader)!;
                }
            }

            return new PaginationMetadata();
        }
    }
}
