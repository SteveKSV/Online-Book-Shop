using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetOrderById : IRequest<Order>
    {
        public Guid Id { get; set; }
    }
}
