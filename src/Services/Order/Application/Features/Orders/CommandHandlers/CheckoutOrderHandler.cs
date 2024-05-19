using Application.Dtos;
using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.CommandHandlers
{
    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrder, OrderDto>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public CheckoutOrderHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(CheckoutOrder request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request.OrderDto);

            var createdOrder = await _repository.CheckoutOrder(order);
            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            if (orderDto != null)
            {
                return orderDto;
            }
            else
            {
                throw new Exception("Created order (from repository method) is null");
            }
        }
    }
}
