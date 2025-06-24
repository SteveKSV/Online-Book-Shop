using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public record GetOrdersByUserId(Guid UserId) : IRequest<List<OrderDto>>;
}
