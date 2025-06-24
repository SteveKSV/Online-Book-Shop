using AutoMapper;
using Catalog.DTO;
using Catalog.Entities;
using Catalog.Helpers;
using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Catalog.Managers
{
    public class BookManager : GenericManager<Book>, IBookManager
    {
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public BookManager(AppDbContext context, IMapper mapper, IDistributedCache cache) : base(context)
        {
            _mapper = mapper;
            _cache = cache;
        }

        private async Task<T?> GetOrSetCacheAsync<T>(string key, Func<Task<T>> getData, TimeSpan? expiry = null)
        {
            try
            {
                var cached = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cached))
                {
                    return JsonSerializer.Deserialize<T>(cached);
                }

                var data = await getData();
                var jsonData = JsonSerializer.Serialize(data);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(key, jsonData, options);
                return data;
            }
            catch
            {
                // Якщо Redis або інший кеш не доступний — повертаємо напряму з бази
                return await getData();
            }
        }

        public async Task<PagedList<BookDTO?>> GetBooks(PaginationParams? paginationParams, string? title, string? sortOrder, string? genre, string? sortRating)
        {
            string cacheKey = $"books:{paginationParams?.PageNumber}:{paginationParams?.PageSize}:{title}:{sortOrder}:{genre}:{sortRating}";

            var cached = await GetOrSetCacheAsync($"books:{paginationParams?.PageNumber}:{paginationParams.PageSize}:{title}:{sortOrder}:{genre}:{sortRating}", async () =>
            {
                var query = _dbSet.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(b => EF.Functions.Like(b.Title, $"%{title}%"));
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    query = query.Where(b => b.Genre.Name == genre);
                }

                if (sortOrder?.ToLower() == "asc")
                    query = query.OrderBy(b => b.Price).ThenBy(b => b.Title);
                else if (sortOrder?.ToLower() == "desc")
                    query = query.OrderByDescending(b => b.Price).ThenByDescending(b => b.Title);

                if (sortRating?.ToLower() == "asc")
                    query = query.OrderBy(b => b.AverageRating).ThenBy(b => b.Title);
                else if (sortRating?.ToLower() == "desc")
                    query = query.OrderByDescending(b => b.AverageRating).ThenByDescending(b => b.Title);

                var totalCount = await query.CountAsync();

                var books = await query
                    .Skip((paginationParams!.PageNumber - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToListAsync();

                var bookDtos = _mapper.Map<List<BookDTO>>(books);
                var paged = new PagedList<BookDTO>(bookDtos, totalCount, paginationParams.PageNumber, paginationParams.PageSize);

                return CachedPagedList<BookDTO?>.FromPagedList(paged!);
            });

            return cached!.ToPagedList();
        }

        public async Task<BookDTO?> GetBookByTitle(string title)
        {
            string cacheKey = $"book:title:{title}";

            return await GetOrSetCacheAsync(cacheKey, async () =>
            {
                var book = await _dbSet.Include(c => c.Genre)
                                       .FirstOrDefaultAsync(b => b.Title == title);
                return _mapper.Map<BookDTO?>(book);
            });
        }

        public async Task<BookDTO?> GetBookById(Guid id)
        {
            string cacheKey = $"book:id:{id}";

            return await GetOrSetCacheAsync(cacheKey, async () =>
            {
                var book = await _dbSet
                    .Include(b => b.Genre)
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.Id == id);

                return _mapper.Map<BookDTO?>(book);
            });
        }

        public async Task<PagedList<CommentDTO>> GetCommentsForBook(Guid bookId, PaginationParams paginationParams)
        {
            var query = _context.Comments.Include(c => c.User)
                .Where(c => c.BookId == bookId)
                .OrderByDescending(c => c.CommentedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var comments = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<CommentDTO>>(comments);
            return new PagedList<CommentDTO>(commentDtos, totalCount, paginationParams.PageNumber, paginationParams.PageSize);
        }


        public async Task<BookDTO> CreateBookAsync(BookCreateDTO newBookDto)
        {
            try
            {
                var book = _mapper.Map<Book>(newBookDto);
                book.Id = Guid.NewGuid();
                book.AverageRating = 3;

                await _dbSet.AddAsync(book);
                await _context.SaveChangesAsync();

                await _context.Entry(book).Reference(b => b.Genre).LoadAsync();

                return _mapper.Map<BookDTO>(book);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create book.", ex);
            }
        }

        public async Task<BookDTO?> UpdateBookAsync(BookCreateDTO updatedBookDto)
        {
            try
            {
                if (updatedBookDto.Id == Guid.Empty)
                    throw new ArgumentException("Id книги має бути вказаний для оновлення.");

                var existingBook = await _dbSet
                    .Include(b => b.Genre)
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.Id == updatedBookDto.Id);

                if (existingBook == null)
                    return null;

                _mapper.Map(updatedBookDto, existingBook);
                await _context.SaveChangesAsync();

                // Очистка кешу після оновлення
                var cacheKey = $"book:id:{updatedBookDto.Id}";
                try
                {
                    await _cache.RemoveAsync(cacheKey);
                }
                catch { }

                return _mapper.Map<BookDTO>(existingBook);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update book.", ex);
            }
        }

        public async Task<bool> DeleteEntity(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                    return false;

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();

                // Очистка кешу після видалення
                var cacheKey = $"book:id:{id}";
                try
                {
                    await _cache.RemoveAsync(cacheKey);
                }
                catch { }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete book with id '{id}'.", ex);
            }
        }
    }
}
