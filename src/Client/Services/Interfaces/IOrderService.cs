using Client.Models;

namespace Client.Services.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrder(BasketCheckout order);
    }
}
