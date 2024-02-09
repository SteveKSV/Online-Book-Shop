using Application.Features.Orders.Queries;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Features.Orders.QueryHandlers
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrders, List<Order>>
    {
        private readonly IOrderRepository _repository;

        public GetAllOrdersHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Order>> Handle(GetAllOrders request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllOrders();
        }
    }
}
