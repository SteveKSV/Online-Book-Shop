using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.CommandHandlers
{
    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrder, Order>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        public CheckoutOrderHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Order> Handle (CheckoutOrder request, CancellationToken cancellationToken)
        {
            Order order = _mapper.Map<Order>(request);

            var createdOrder = await _repository.CheckoutOrder(order);

            if (createdOrder != null)
            {
                return createdOrder;
            }
            else
            {
                throw new Exception("Created order (from repository method) is null");
            }
        }
    }
}
