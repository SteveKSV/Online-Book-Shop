using Application.Features.Orders.Commands;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _dbContext;

        public OrderRepository(OrderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Status)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .ToListAsync();

            return orders ?? throw new Exception($"No orders found.");
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            var order = await _dbContext.Orders
                 .Include(o => o.Status)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PaymentMethod)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order ?? throw new Exception($"Order with ID {id} not found.");
        }

        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _dbContext.Orders
                 .Where(o => o.UserId == userId)
                 .Include(o => o.Items)
                     .ThenInclude(oi => oi.Book)
                 .Include(o => o.Status)
                 .Include(o => o.Payment)
                     .ThenInclude(p => p.PaymentMethod)
                 .Include(o => o.Payment)
                     .ThenInclude(p => p.CardPayments)
                 .OrderByDescending(o => o.CreatedAt) 
                 .ToListAsync();

            return orders;
        }


        public async Task<Order> CheckoutOrder(Order order)
        {
            // 1. Перевірка наявності всіх книг
            var bookIds = order.Items.Select(i => i.BookId).Distinct().ToList();

            var existingBookIds = await _dbContext.Books
                .Where(b => bookIds.Contains(b.Id))
                .Select(b => b.Id)
                .ToListAsync();

            var missingBooks = bookIds.Except(existingBookIds).ToList();
            if (missingBooks.Any())
            {
                throw new Exception($"Order contains BookIds that do not exist in the database: {string.Join(", ", missingBooks)}");
            }

            // 2. Обробка платіжної інформації (картки)
            if (order.Payment != null && order.Payment.CardPayments != null)
            {
                // Фільтруємо тільки ті картки, які мають всі потрібні поля
                order.Payment.CardPayments = order.Payment.CardPayments
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CardNumber)
                        && !string.IsNullOrWhiteSpace(c.Cvv)
                        && c.ExpiryMonth > 0 && c.ExpiryMonth <= 12
                        && c.ExpiryYear >= DateTime.UtcNow.Year 
                    )
                    .ToList();

                // Якщо жодної валідної картки — очищаємо список
                if (!order.Payment.CardPayments.Any())
                {
                    order.Payment.CardPayments = null;
                }
            }

            // 3. Додаємо замовлення
            await _dbContext.Orders.AddAsync(order);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                throw new Exception($"Cannot create order: {ex.Message}. Inner exception: {innerMessage}");
            }

            // 4. Завантажуємо повні дані
            var orderWithDetails = await _dbContext.Orders
                .Include(o => o.Payment)
                    .ThenInclude(p => p.PaymentMethod)
                .Include(o => o.Payment)
                    .ThenInclude(p => p.CardPayments)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return orderWithDetails!;
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            // Спочатку видаляємо payment
            if (order.Payment != null)
                _dbContext.Remove(order.Payment);

            _dbContext.OrderItems.RemoveRange(order.Items);
            _dbContext.Orders.Remove(order);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            var existingOrder = await _dbContext.Orders
                .Include(o => o.Items)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
                throw new Exception($"Order with ID {order.Id} not found.");

            // Оновлюємо основні поля
            _dbContext.Entry(existingOrder).CurrentValues.SetValues(order);

            // Оновлення Items
            _dbContext.OrderItems.RemoveRange(existingOrder.Items);
            foreach (var item in order.Items)
            {
                existingOrder.Items.Add(item);
            }

            // Оновлення Payment
            if (existingOrder.Payment != null)
            {
                _dbContext.Entry(existingOrder.Payment).CurrentValues.SetValues(order.Payment);
            }
            else if (order.Payment != null)
            {
                existingOrder.Payment = order.Payment;
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        // Якщо треба буде — наприклад для міграції старих даних — оновити всі UserId
        public async Task<bool> UpdateUserIdInOrders(Guid oldUserId, Guid newUserId)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.UserId == oldUserId)
                .ToListAsync();

            if (!orders.Any()) return false;

            foreach (var order in orders)
            {
                order.UserId = newUserId;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }

}
