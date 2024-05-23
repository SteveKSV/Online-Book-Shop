using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Commands
{
    public class CheckoutOrder : IRequest<OrderDto>
    {
        public AddOrderDto OrderDto { get; set; }
    }
}
