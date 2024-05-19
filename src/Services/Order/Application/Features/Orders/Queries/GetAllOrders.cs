using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetAllOrders : IRequest<List<OrderDto>>
    {
    }
}
