using Basket.Data;
using Basket.DTO;
using Basket.Entities;
using Basket.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Basket.Managers
{
    public class BasketManager : IBasketManager
    {
        private readonly BasketDbContext _context;
        private readonly ICatalogService _catalogService;

        public BasketManager(BasketDbContext context, ICatalogService catalogService)
        {
            _context = context;
            _catalogService = catalogService;
        }

        public async Task<BasketDTO> GetBasket(Guid userId)
        {
            var items = await _context.BasketItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = new List<BasketItemDetailedDTO>();

            foreach (var item in items)
            {
                var book = await _catalogService.GetBookByIdAsync(item.BookId);

                result.Add(new BasketItemDetailedDTO
                {
                    BookId = item.BookId,
                    Title = book?.Title ?? "Unknown",
                    Authors = book?.Authors ?? "Unknown",
                    Quantity = item.Quantity,
                    Price = item.Price,
                    CoverImage = book?.CoverImage ?? "https://www.forewordreviews.com/books/covers/the-official-librarian.jpg"
                });
            }

            var total = result.Sum(x => x.Price * x.Quantity);

            return new BasketDTO
            {
                Items = result,
                TotalPrice = total
            };
        }

        public async Task<BasketItem> AddOrUpdateItem(Guid userId, Guid bookId, int quantity, decimal price)
        {
            var item = await _context.BasketItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);

            if (item == null)
            {
                item = new BasketItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    Price = price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.BasketItems.Add(item);
            }
            else
            {
                item.Quantity += quantity;
                item.Price = price;
                item.UpdatedAt = DateTime.UtcNow;
                _context.BasketItems.Update(item);
            }

            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<BasketItem> UpdateItemQuantity(Guid userId, Guid bookId, int quantity)
        {
            var item = await _context.BasketItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (item == null) return null;

            item.Quantity = quantity;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteBasket(Guid userId)
        {
            var items = _context.BasketItems.Where(x => x.UserId == userId);
            _context.BasketItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<BasketDTO> RemoveItemFromBasket(Guid userId, Guid bookId)
        {
            var item = await _context.BasketItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return await GetBasket(userId);
        }
    }

}
