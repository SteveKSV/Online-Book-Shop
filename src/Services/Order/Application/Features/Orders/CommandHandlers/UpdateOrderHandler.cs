using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.CommandHandlers
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrder, bool>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        public UpdateOrderHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateOrder request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request.Order);

            if (order != null)
            {
                return await _repository.UpdateOrder(order);
            }

            throw new Exception($"Order is null in Handler (UpdateOrderHandler)");
        }
    }
}
