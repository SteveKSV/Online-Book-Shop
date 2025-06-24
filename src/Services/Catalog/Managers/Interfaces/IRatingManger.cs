using Catalog.DTO;
using Catalog.Helpers;

namespace Catalog.Managers.Interfaces
{
    public interface IRatingManager
    {
        Task<PagedList<RatingDTO>> GetAllRatings(int pageNumber, int pageSize);
        Task<int> GetUserRating(Guid bookId, Guid userId);
        Task<RatingDTO> RateAsync(Guid userId, Guid bookId, int value);
        Task<RatingDTO> UpdateRatingAsync(Guid userId, Guid bookId, int newValue);
        Task<bool> DeleteRatingAsync(Guid userId, Guid bookId);
    }
}
