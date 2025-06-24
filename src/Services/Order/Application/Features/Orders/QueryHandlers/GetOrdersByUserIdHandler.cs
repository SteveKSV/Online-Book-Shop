using Application.Dtos;
using Application.Features.Orders.Queries;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.QueryHandlers
{
    public class GetOrdersByUserIdHandler : IRequestHandler<GetOrdersByUserId, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersByUserIdHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByUserId request, CancellationToken cancellationToken)
        {
            var orderList = await _orderRepository.GetOrdersByUserId(request.UserId);
            return _mapper.Map<List<OrderDto>>(orderList);
        }
    }
}
