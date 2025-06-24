using Catalog.DTO;
using Catalog.Entities;
using Catalog.Helpers;

namespace Catalog.Managers.Interfaces
{
    public interface IBookManager : IGenericManager<Book>
    {
        Task<PagedList<BookDTO?>> GetBooks(PaginationParams? paginationParams, string? title, string? sortOrder, string? genre, string? sortRating);
        Task<BookDTO?> GetBookById(Guid id);
        Task<BookDTO?> GetBookByTitle(string title);
        Task<PagedList<CommentDTO>> GetCommentsForBook(Guid bookId, PaginationParams paginationParams);
        Task<BookDTO> CreateBookAsync(BookCreateDTO newBookDto);
        Task<BookDTO> UpdateBookAsync(BookCreateDTO newBookDto);
        
    }
}
