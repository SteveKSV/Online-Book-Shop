using Client.Models.Catalog;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<(List<BookModel>, PaginationMetadata)> GetBooks(string? queryString = null);
        Task<List<string>> Get(string? queryString = null);
        Task<(List<Comment>, PaginationMetadata)> GetCommentsForBook(Guid bookId, string? queryString = null);
        Task<bool> AddCommentAsync(AddUpdateComment comment);
        Task<bool> UpdateCommentAsync(AddUpdateComment comment);
        Task<bool> DeleteCommentAsync(Guid commentId);

    }
}
