using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetOrderById : IRequest<OrderDto>
    {
        public Guid Id { get; set; }
    }
}
