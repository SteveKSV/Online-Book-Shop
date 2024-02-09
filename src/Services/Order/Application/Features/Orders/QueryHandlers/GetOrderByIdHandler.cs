using Application.Features.Orders.Queries;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.QueryHandlers
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderById, Order>
    {
        private readonly IOrderRepository _repository;

        public GetOrderByIdHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            return await _repository.GetOrderById(request.Id);
        }
    }
}
