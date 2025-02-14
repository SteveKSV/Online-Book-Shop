using Catalog.Entities;
using Catalog.Helpers;

namespace Catalog.Managers.Interfaces
{
    public interface IReviewManager : IGenericManager<Review>
    {
        Task<PagedList<Review?>> GetAllReviewsAsync(PaginationParams? paginationParams);
        Task<Review> GetReviewById(string id);
        Task<IEnumerable<Review>> GetAllReviewsByBookId(string bookId);
    }
}
