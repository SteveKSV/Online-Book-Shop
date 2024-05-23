using Application.Dtos;
using Application.Features.Orders.Queries;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.QueryHandlers
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderById, OrderDto>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetOrderById(request.Id);
            return _mapper.Map<OrderDto>(orders);
        }
    }
}
