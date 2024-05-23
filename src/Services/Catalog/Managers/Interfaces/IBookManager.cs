using Catalog.Entities;
using Catalog.Helpers;

namespace Catalog.Managers.Interfaces
{
    public interface IBookManager : IGenericManager<Book>
    {
        Task<PagedList<Book?>> GetBooks(PaginationParams? paginationParams, string? title, string? sortOrder, List<string>? genres, List<string>? languages);
        Task<Book> GetBookById(string id);
        Task<Book> GetBookByTitle(string title);

    }
}
