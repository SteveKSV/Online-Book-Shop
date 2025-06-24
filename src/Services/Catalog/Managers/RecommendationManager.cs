using AutoMapper;
using Catalog.DTO;
using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Catalog.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public RecommendationManager(AppDbContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<RecommendedBookDto>> GetTopBooksAsync()
        {
            const string cacheKey = "top_books_redis";

            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<List<RecommendedBookDto>>(cached)!;
            }
            catch { }

            var topCount = 10;

            var books = await _context.Books
                .Include(b => b.Genre)
                .Select(b => new RecommendedBookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = b.Authors,
                    CoverImage = b.CoverImage,
                    Genre = b.Genre.Name,
                    Price = b.Price,
                    AverageRating = _context.Ratings
                        .Where(r => r.BookId == b.Id)
                        .Select(r => (float?)r.Value)
                        .Average() ?? 0f,
                    RelevanceScore = 1f
                })
                .OrderByDescending(b => b.AverageRating)
                .ThenBy(b => b.Title)
                .Take(topCount)
                .ToListAsync();

            try
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                var json = JsonSerializer.Serialize(books);
                await _cache.SetStringAsync(cacheKey, json, options);
            }
            catch { }

            return books;
        }

        public async Task<List<RecommendedBookDto>> GetNewBooksAsync()
        {
            const string cacheKey = "new_books_redis";

            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<List<RecommendedBookDto>>(cached)!;
            }
            catch { }

            var books = await _context.Books
                .Include(b => b.Genre)
                .OrderByDescending(b => b.PublishedAt)
                .Take(10)
                .Select(b => new RecommendedBookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = b.Authors,
                    CoverImage = b.CoverImage,
                    Genre = b.Genre.Name,
                    Price = b.Price,
                    AverageRating = b.AverageRating,
                    RelevanceScore = 0.5f
                })
                .ToListAsync();

            try
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                var json = JsonSerializer.Serialize(books);
                await _cache.SetStringAsync(cacheKey, json, options);
            }
            catch { }

            return books;
        }
        public async Task<List<RecommendedBookDto>> GetRecommendationsAsync(Guid userId, int top = 10)
        {
            string cacheKey = $"recommendations_{userId}_{top}";

            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<List<RecommendedBookDto>>(cached)!;
            }
            catch { }

            var ratedBookIds = await _context.Ratings.Where(r => r.UserId == userId).Select(r => r.BookId).ToListAsync();
            var orderedBookIds = await _context.OrderItems.Where(o => o.Order!.UserId == userId).Select(o => o.BookId).ToListAsync();

            var hasHistory = ratedBookIds.Any() || orderedBookIds.Any();

            List<RecommendedBookDto> recommendations;

            if (hasHistory)
            {
                var cbf = await GetContentBasedRecommendationsAsync(userId, top * 2);
                var cf = await GetCollaborativeFilteringRecommendationsAsync(userId, top * 2);

                var combined = cbf.Concat(cf)
                    .GroupBy(b => b.Id)
                    .Select(g => new RecommendedBookDto
                    {
                        Id = g.Key,
                        Title = g.First().Title,
                        Genre = g.First().Genre,
                        AverageRating = g.Average(x => x.AverageRating),
                        RelevanceScore = g.Average(x => x.RelevanceScore),
                        Authors = g.First().Authors,
                        CoverImage = g.First().CoverImage,
                        Price = g.First().Price
                    })
                    .OrderByDescending(x => x.RelevanceScore)
                    .ThenByDescending(x => x.AverageRating)
                    .Take(top)
                    .ToList();

                if (combined.Count < top)
                {
                    var topBooks = await GetTopBooksAsync();
                    var additional = topBooks.Where(b => !combined.Any(c => c.Id == b.Id)).Take(top - combined.Count);
                    combined.AddRange(additional);
                }

                recommendations = combined;
            }
            else
            {
                recommendations = await GetTopBooksAsync();
            }

            try
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                var json = JsonSerializer.Serialize(recommendations);
                await _cache.SetStringAsync(cacheKey, json, options);
            }
            catch { }

            return recommendations;
        }

        private async Task<List<RecommendedBookDto>> GetContentBasedRecommendationsAsync(Guid userId, int top)
        {
            var orderedBookIds = await _context.OrderItems.Where(o => o.Order.UserId == userId).Select(o => o.BookId).ToListAsync();

            var userGenrePreferences = await _context.Books
                .Where(b => orderedBookIds.Contains(b.Id))
                .GroupBy(b => b.GenreId)
                .Select(g => new { GenreId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(2)
                .ToListAsync();

            var genreIds = userGenrePreferences.Select(g => g.GenreId).ToList();

            var books = await _context.Books
                .Where(b => genreIds.Contains(b.GenreId) && !orderedBookIds.Contains(b.Id))
                .Include(b => b.Genre)
                .Select(b => new RecommendedBookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Genre = b.Genre.Name,
                    Authors = b.Authors,
                    CoverImage = b.CoverImage,
                    Price = b.Price,
                    AverageRating = _context.Ratings.Where(r => r.BookId == b.Id).Select(r => (float?)r.Value).Average() ?? 0f,
                    RelevanceScore = 0.7f
                })
                .Take(top)
                .ToListAsync();

            return books;
        }

        private async Task<List<RecommendedBookDto>> GetCollaborativeFilteringRecommendationsAsync(Guid userId, int top)
        {
            var userBookIds = await _context.Ratings.Where(r => r.UserId == userId).Select(r => r.BookId).Distinct().ToListAsync();
            if (!userBookIds.Any()) return new List<RecommendedBookDto>();

            var similarUsers = await _context.Ratings
                .Where(r => userBookIds.Contains(r.BookId) && r.UserId != userId)
                .GroupBy(r => r.UserId)
                .Select(g => new { UserId = g.Key, Similarity = g.Count() })
                .OrderByDescending(x => x.Similarity)
                .Take(5)
                .ToListAsync();

            var similarUserIds = similarUsers.Select(u => u.UserId).ToList();
            if (!similarUserIds.Any()) return new List<RecommendedBookDto>();

            var weightedRatings = await _context.Ratings
                .Where(r => similarUserIds.Contains(r.UserId) && !userBookIds.Contains(r.BookId))
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    WeightedScore = g.Average(r => r.Value)
                })
                .OrderByDescending(x => x.WeightedScore)
                .Take(top)
                .ToListAsync();

            var books = await _context.Books
                .Where(b => weightedRatings.Select(w => w.BookId).Contains(b.Id))
                .Include(b => b.Genre)
                .ToListAsync();

            var result = books.Select(b => new RecommendedBookDto
            {
                Id = b.Id,
                Title = b.Title,
                Genre = b.Genre.Name,
                Authors = b.Authors,
                CoverImage = b.CoverImage,
                Price = b.Price,
                AverageRating = weightedRatings.First(w => w.BookId == b.Id).WeightedScore,
                RelevanceScore = 0.9f
            }).ToList();

            return result;
        }
    }
}
