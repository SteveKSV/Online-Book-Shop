using Client.Models.Ordering;

namespace Client.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllOrdersByUserId(Guid userId);
        Task PlaceOrder(BasketCheckout order);
        Task<List<PaymentMethod>> GetPaymentMethods();
    }
}
