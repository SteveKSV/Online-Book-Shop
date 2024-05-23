using Client.Models;

namespace Client.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<(List<BookModel>, PaginationMetadata)> GetBooks(string? queryString = null);
        Task<List<string>> Get(string? queryString = null);

    }
}
