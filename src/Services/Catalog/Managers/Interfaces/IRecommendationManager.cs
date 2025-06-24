using Catalog.DTO;

namespace Catalog.Managers.Interfaces
{
    public interface IRecommendationManager
    {
        Task<List<RecommendedBookDto>> GetRecommendationsAsync(Guid userId, int top = 10); 
        Task<List<RecommendedBookDto>> GetTopBooksAsync();
        Task<List<RecommendedBookDto>> GetNewBooksAsync();
    }
}
