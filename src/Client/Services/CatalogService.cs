using Client.Models;
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
            //// Отримання токену доступу
            //var token = await _tokenService.GetToken("Catalog.read");

            // Створення HTTP запиту
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.GetSection("apiUrl").Value}/catalog{queryString}");

            //Встановлення заголовка авторизації з використанням токену
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            // Виконання запиту
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode) return (null, null);

            // Retrieve books from response body
            var books = await response.Content.ReadFromJsonAsync<List<BookModel>>();

            // Retrieve pagination metadata from response headers
            var paginationMetadata = ParsePaginationMetadata(response.Headers);

            return (books, paginationMetadata);

        }

        public async Task<List<string>> Get(string queryString = null)
        {
            var response = await _httpClient.GetAsync($"{_configuration.GetSection("apiUrl").Value}/{queryString}");

            if (!response.IsSuccessStatusCode) return null;

            // Retrieve entities from response body
            var entities = await response.Content.ReadFromJsonAsync<List<string>>();

            return entities;

        }
        private PaginationMetadata ParsePaginationMetadata(HttpResponseHeaders headers)
        {
            PaginationMetadata metadata = new PaginationMetadata();

            if (headers.Contains("X-Pagination"))
            {
                var paginationHeader = headers.GetValues("X-Pagination").FirstOrDefault();
                if (!string.IsNullOrEmpty(paginationHeader))
                {
                    var paginationData = JsonConvert.DeserializeObject<dynamic>(paginationHeader);
                    metadata.TotalCount = paginationData!.TotalCount;
                    metadata.PageSize = paginationData.PageSize;
                    metadata.CurrentPage = paginationData.CurrentPage;
                    metadata.TotalPages = paginationData.TotalPages;
                    metadata.HasNext = paginationData.HasNext;
                    metadata.HasPrevious = paginationData.HasPrevious;
                }
            }

            return metadata;
        }

    }
}
