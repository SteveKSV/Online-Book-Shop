using Application.Features.Orders.Commands;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(Guid id);
        Task<List<Order>> GetOrdersByUserId(Guid userId);
        Task<Order> CheckoutOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Guid id);
    }
}
