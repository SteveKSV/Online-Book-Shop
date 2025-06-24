using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Catalog.Managers
{
    public class GenreManager : IGenreManager
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public GenreManager(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<string>> GetAllGenres()
        {
            string cacheKey = "genre_list";

            try
            {
                var cachedGenres = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedGenres))
                {
                    return JsonSerializer.Deserialize<List<string>>(cachedGenres)!;
                }
            }
            catch
            {
                // Якщо кеш недоступний — ігноруємо помилку і працюємо напряму з бази
            }

            var genres = await _context.Genres
                .Select(g => g.Name)
                .Distinct()
                .ToListAsync();

            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(genres), options);
            }
            catch
            {
                // Ігноруємо помилки при записі кешу
            }

            return genres;
        }
    }
}
