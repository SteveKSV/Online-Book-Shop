using Application.Dtos;
using MediatR;

namespace Application.Features.Orders.Commands
{
    public class UpdateOrder : IRequest<bool>
    {
        public UpdateOrderDto Order { get; set; }
    }
}
