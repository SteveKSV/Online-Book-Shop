using Catalog.Entities;
using Catalog.Managers.Interfaces;
using MongoDB.Driver;

namespace Catalog.Managers
{
    public class GenreManager : GenericManager<Book>, IGenreManager
    {
        public GenreManager(MongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<string>> GetAllGenres()
        {
            // Use the Distinct method to get unique genres from all documents
            var genres = await _collection.Distinct<string>("Genre", FilterDefinition<Book>.Empty).ToListAsync();

            // Split each genre string by comma, filter out empty strings, and flatten the list
            var allGenres = genres
                .SelectMany(g => g.Split(',')) // Split each genre string by comma
                .Select(g => g.Trim()) // Trim leading and trailing spaces
                .Where(g => !string.IsNullOrEmpty(g)) // Filter out empty strings
                .Distinct() // Get unique genres
                .ToList();

            return allGenres;
        }

    }
}
