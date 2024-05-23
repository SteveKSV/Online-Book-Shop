using Application.Features.Orders.Commands;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(Guid id);
        Task<Order> CheckoutOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Guid id);

        Task<List<Order>> GetOrdersByUsername(string username);
        Task<bool> UpdateUserNameInOrders(UpdateUsername updateUsername);
    }
}
