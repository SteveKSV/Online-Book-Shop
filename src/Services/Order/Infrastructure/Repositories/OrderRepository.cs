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
            List<Order> orders = await _dbContext.Orders.ToListAsync();

            return orders ?? throw new Exception($"GetAllOrders - Order Repository -> not found");
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            Order order = await _dbContext.Orders.FindAsync(id);

            return order ?? throw new Exception($"GetOrderById - Order Repository -> Id: {id} wasn't found");
        }

        public async Task<List<Order>> GetOrdersByUsername(string userName)
        {
            var orderList = await _dbContext.Orders
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
            Order order = await _dbContext.Orders.FindAsync(id);

            if (order != null)
            {
                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new ArgumentException("Order not found (Delete method)", nameof(id));
            }
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            try
            {
                _dbContext.Entry(order).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException) // Handle concurrency conflicts
            {
                // Log the exception details
                throw new Exception($"Concurrency conflict occurred while updating order with ID: {order.Id}");
            }
            catch (DbUpdateException ex) // Handle other database update errors
            {
                // Log the exception details
                throw new Exception($"Error occurred while updating order: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception details
                throw new Exception($"Unexpected error occurred while updating order: {ex.Message}");
            }

        }
    }
}
