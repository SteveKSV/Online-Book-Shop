using Catalog.Entities;
using Catalog.Helpers;

namespace Catalog.Managers.Interfaces
{
    public interface IBookManager : IGenericManager<Book>
    {
        Task<PagedList<Book?>> GetBooks(PaginationParams? paginationParams, string? title, string? sortOrder, List<string>? genres, List<string>? languages);
        Task<Book> GetBookById(string id);
        Task<IEnumerable<Book>> GetBooksByAuthor(string authorName);
        Task<IEnumerable<Book>> GetBooksByPublisher(string publisherName);
        Task<IEnumerable<Book>> GetBooksByGenre(string genre);
        Task<Book> GetBookByTitle(string title);

        Task<int> GetBooksCount();
    }
}
