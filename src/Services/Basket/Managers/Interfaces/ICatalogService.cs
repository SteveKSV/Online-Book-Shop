using Basket.DTO;

namespace Basket.Managers.Interfaces
{
    public interface ICatalogService
    {
        Task<BookDTO?> GetBookByIdAsync(Guid bookId);
    }
}
