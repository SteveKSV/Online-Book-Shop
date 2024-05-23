using Application.Dtos;
using Application.Features.Orders.Queries;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Features.Orders.QueryHandlers
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrders, List<OrderDto>>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public GetAllOrdersHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetAllOrders request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetAllOrders();
            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}
