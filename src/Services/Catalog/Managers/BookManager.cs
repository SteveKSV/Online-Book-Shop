using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Managers
{
    public class BookManager : GenericManager<Book>, IBookManager
    {
        public BookManager(MongoDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Book?>> GetBooks(
            PaginationParams? paginationParams, string? title, string? sortOrder,
            string genre)
        {
            var filterBuilder = new FilterDefinitionBuilder<Book>();
            var filter = filterBuilder.Empty;

            // Filter by title if provided
            if (!string.IsNullOrEmpty(title))
            {
                filter &= filterBuilder.Regex(x => x.Title, new BsonRegularExpression(title, "i"));
            }

            // Filter by genre if provided
            if (!string.IsNullOrEmpty(genre))
            {
                filter &= filterBuilder.Regex(x => x.genres, new BsonRegularExpression(genre, "i"));
            }

            // Create the query with the filter
            var query = _collection.Find(filter);

            try
            {
                // Get total count for pagination
                var totalCount = await _collection.CountDocumentsAsync(filter);

                // Add sorting
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    switch (sortOrder.ToLower())
                    {
                        case "asc":
                            query = query.SortBy(book => book.Price);
                            break;
                        case "desc":
                            query = query.SortByDescending(book => book.Price);
                            break;
                    }
                }

                // Apply pagination
                var books = await query
                    .Skip((paginationParams!.PageNumber - 1) * paginationParams.PageSize)
                    .Limit(paginationParams.PageSize)
                    .ToListAsync();

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
