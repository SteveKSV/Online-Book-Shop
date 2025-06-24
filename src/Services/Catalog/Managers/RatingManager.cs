using AutoMapper;
using Catalog.DTO;
using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Managers
{
    public class RatingManager : IRatingManager
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RatingManager(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<RatingDTO>> GetAllRatings(int pageNumber, int pageSize)
        {
            var source = _context.Ratings.AsNoTracking();

            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoItems = _mapper.Map<List<RatingDTO>>(items);

            return new PagedList<RatingDTO>(dtoItems, count, pageNumber, pageSize);
        }
        public async Task<int> GetUserRating(Guid bookId, Guid userId)
        {
            if (bookId == Guid.Empty || userId == Guid.Empty || bookId == null || userId == null)
            {
                throw new ArgumentException("Book ID and User ID must be valid GUIDs.");
            }

            var rating = await _context.Ratings
                .Where(r => r.BookId == bookId && r.UserId == userId)
                .Select(r => r.Value)
                .FirstOrDefaultAsync();

            return rating;
        }

        public async Task<RatingDTO> RateAsync(Guid userId, Guid bookId, int value)
        {
            try
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 5.");

                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);

                if (existingRating is null)
                {
                    var rating = new Rating
                    {
                        UserId = userId,
                        BookId = bookId,
                        Value = value,
                        RatedAt = DateTime.UtcNow
                    };

                    _context.Ratings.Add(rating);
                    await _context.SaveChangesAsync();
                    await UpdateBookAverageRating(bookId);
                    return _mapper.Map<RatingDTO>(rating);
                }
                else
                {
                    existingRating.Value = value;
                    existingRating.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await UpdateBookAverageRating(bookId);
                    return _mapper.Map<RatingDTO>(existingRating);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to rate the book.", ex);
            }
        }

        public async Task<RatingDTO> UpdateRatingAsync(Guid userId, Guid bookId, int newValue)
        {
            try
            {
                if (newValue < 1 || newValue > 5)
                    throw new ArgumentOutOfRangeException(nameof(newValue), "Rating must be between 1 and 5.");

                var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
                if (rating == null)
                    throw new KeyNotFoundException("Rating not found.");

                rating.Value = newValue;
                rating.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return _mapper.Map<RatingDTO>(rating);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update the rating.", ex);
            }
        }

        public async Task<bool> DeleteRatingAsync(Guid userId, Guid bookId)
        {
            try
            {
                var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
                if (rating == null)
                    return false;

                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete the rating.", ex);
            }
        }

        private async Task UpdateBookAverageRating(Guid bookId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .Select(r => (float?)r.Value)
                .ToListAsync();

            // Якщо лише один рейтинг — додаємо штучний 3.0
            if (ratings.Count == 1)
            {
                ratings.Add(3.0f);
            }

            // Якщо взагалі немає оцінок — встановлюємо середнє як 3.0
            var average = ratings.Any() ? ratings.Average() ?? 3.0f : 3.0f;

            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.AverageRating = average;
                await _context.SaveChangesAsync();
            }
        }

    }
}
