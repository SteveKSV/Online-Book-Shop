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
            List<Order> orders = await _dbContext.Orders.Include(o => o.Items).ToListAsync();

            return orders ?? throw new Exception($"GetAllOrders - Order Repository -> not found");
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            Order order = await _dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

            return order ?? throw new Exception($"GetOrderById - Order Repository -> Id: {id} wasn't found");
        }

        public async Task<List<Order>> GetOrdersByUsername(string userName)
        {
            var orderList = await _dbContext.Orders
                .Include(o => o.Items)
                .Where(o => o.UserName == userName)
                .ToListAsync();
            return orderList;
        }
        public async Task<Order> CheckoutOrder(Order order)
        {
            await _dbContext.AddAsync(order);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception details
                throw new Exception($"Cannot create order: {ex.Message}");
            }
            return order;
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            try
            {
                // Знаходимо замовлення за його ідентифікатором
                var order = await _dbContext.Orders.FindAsync(id);

                if (order == null)
                {
                    // Замовлення не знайдено
                    return false;
                }

                // Видаляємо всі елементи зв'язаних даних (OrderItems) для цього замовлення
                _dbContext.OrderItems.RemoveRange(order.Items);

                // Видаляємо саме замовлення
                _dbContext.Orders.Remove(order);

                // Зберігаємо зміни в базі даних
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Обробляємо помилку видалення
                // Наприклад, записуємо її до журналу або відправляємо повідомлення про помилку
                throw new Exception($"An error occurred while deleting the order: {ex.Message}");
            }
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            try
            {
                // Завантажуємо замовлення з його елементами з бази даних
                var existingOrder = await _dbContext.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (existingOrder == null)
                {
                    throw new Exception($"Order with ID {order.Id} not found.");
                }

                // Оновлюємо поля замовлення
                _dbContext.Entry(existingOrder).CurrentValues.SetValues(order);

                // Видаляємо всі елементи замовлення
                _dbContext.OrderItems.RemoveRange(existingOrder.Items);

                // Додаємо нові елементи замовлення
                foreach (var item in order.Items)
                {
                    existingOrder.Items.Add(item);
                }

                // Зберігаємо зміни в базі даних
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Concurrency conflict occurred while updating order with ID: {order.Id}");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error occurred while updating order: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error occurred while updating order: {ex.Message}");
            }
        }

        public async Task<bool> UpdateUserNameInOrders(UpdateUsername updateUsername)
        {
            var orders = await GetOrdersByUsername(updateUsername.OldUsername);

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    order.UserName = updateUsername.NewUsername;
                }
            }
          
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating usernames in orders: {ex.Message}");
                return false;
            }
        }
    }
}
