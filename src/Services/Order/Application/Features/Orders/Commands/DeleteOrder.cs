using MediatR;

namespace Application.Features.Orders.Commands
{
    public class DeleteOrder : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
