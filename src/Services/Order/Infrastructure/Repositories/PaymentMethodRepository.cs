using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly OrderContext _dbContext;

        public PaymentMethodRepository(OrderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethods()
        {
            return await _dbContext.PaymentMethods.ToListAsync();
        }
    }
}
