using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Managers
{
    public class BookManager : GenericManager<Book>, IBookManager
    {
        public BookManager(MongoDbContext context) : base(context)
        {
        }
        public async Task<PagedList<Book?>> GetBooks(
            PaginationParams? paginationParams, string? title, string? sortOrder, 
            List<string>? genres, List<string>? languages)
        {
            var filterBuilder = new FilterDefinitionBuilder<Book>();
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(title))
            {
                filter &= filterBuilder.Regex(x => x.Title, new BsonRegularExpression(title, "i"));
            }

            if (genres != null && genres.Any())
            {
                // Створення фільтра для кожного жанру
                var genreFilters = genres.Select(genre => filterBuilder.All(x => x.Genre, genres)).ToList();

                // Об'єднання фільтрів за допомогою оператора And
                filter &= filterBuilder.And(genreFilters);
            }

            if (languages != null && languages.Any())
            {
                // Створення фільтрів для кожного жанру
                var languageFilters = languages.Select(language => filterBuilder.Eq(x => x.LanguageName, language)).ToList();

                // Об'єднання фільтрів за допомогою оператора And
                filter &= filterBuilder.And(languageFilters);
            }

            var query = _collection.Find(filter);

            try
            {
                var totalCount = await _collection.CountDocumentsAsync(filter);
                var books = await query.ToListAsync();

                if (sortOrder != null)
                {
                    switch (sortOrder.ToLower())
                    {
                        case "asc":
                            books = books.OrderBy(book => book.Price).ToList(); break;
                        case "desc":
                            books = books.OrderByDescending(book => book.Price).ToList(); break;
                        case "none":
                            break;
                    }
                }

                // Apply pagination
                books = books
                    .Skip((paginationParams!.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList();

                return new PagedList<Book?>(books!, (int)totalCount, paginationParams.PageNumber, paginationParams.PageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<Book> GetBookByTitle(string title)
        {
            return await _collection
                          .Find(p => p.Title == title)
                          .FirstOrDefaultAsync();
        }

        public async Task<Book> GetBookById(string id)
        {
            return await _collection
                           .Find(p => p.Id == id)
                           .FirstOrDefaultAsync();
        }

    }
}
