using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using MongoDB.Driver;

namespace Catalog.Managers
{
    public class ReviewManager : GenericManager<Review>, IReviewManager
    {
        public ReviewManager(MongoDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Review?>> GetAllReviewsAsync(PaginationParams? paginationParams)
        {
            try
            {
                // Get total count for pagination
                var totalCount = await _collection.CountDocumentsAsync(Builders<Review>.Filter.Empty);
                var query = _collection.Find(Builders<Review>.Filter.Empty);

                // Apply pagination
                var reviews = await query
                    .Skip((paginationParams!.PageNumber - 1) * paginationParams.PageSize)
                    .Limit(paginationParams.PageSize)
                    .ToListAsync();

                return new PagedList<Review?>(reviews!, (int)totalCount, paginationParams.PageNumber, paginationParams.PageSize);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetAllReviewsByBookId(string bookId)
        {
            return await _collection
                           .Find(p=>p.BookId == bookId).ToListAsync();
        }

        public async Task<Review> GetReviewById(string id)
        {
            return await _collection
                           .Find(p => p.Id == id)
                           .FirstOrDefaultAsync();
        }
    }
}
