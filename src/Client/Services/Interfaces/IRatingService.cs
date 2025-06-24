using Client.Models.Catalog.Ratings;

namespace Client.Services.Interfaces
{
    public interface IRatingService
    {
        Task<int?> GetUserRatingAsync(Guid bookId, Guid userId);
        Task<bool> RateBookAsync(RateBook entity);
        Task<bool> UpdateRatingAsync(UpdateRating entity);
    }
}
