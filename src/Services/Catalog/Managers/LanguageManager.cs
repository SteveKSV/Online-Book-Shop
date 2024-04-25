using Catalog.Entities;
using Catalog.Managers.Interfaces;
using MongoDB.Driver;

namespace Catalog.Managers
{
    public class LanguageManager : GenericManager<Book>, ILanguageManager
    {
        public LanguageManager(MongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<string>> GetAllLanguages()
        {
            // Use the Distinct method to get unique languages from all documents
            var languages = await _collection.Distinct<string>("LanguageName", FilterDefinition<Book>.Empty).ToListAsync();

            // Split each language string by comma and flatten the list
            var allLanguages = languages.SelectMany(g => g.Split(',')).Select(g => g.Trim()).Distinct();

            return allLanguages;
        }
    }
}
