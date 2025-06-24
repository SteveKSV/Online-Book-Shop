using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetPaymentMethods();
    }
}
