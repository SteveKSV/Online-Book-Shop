using Catalog.Entities;

namespace Catalog.Managers.Interfaces
{
    public interface IBookManager : IGenericManager<Book>
    {
        Task<IEnumerable<Book?>> GetBooks(string? title, string? author, string? publisher);
        Task<Book> GetBookById(string id);
        Task<IEnumerable<Book>> GetBooksByAuthor(string authorName);
        Task<IEnumerable<Book>> GetBooksByPublisher(string publisherName);
        Task<IEnumerable<Book>> GetBooksByGenre(string genre);
        Task<Book> GetBookByTitle(string title);
    }
}
