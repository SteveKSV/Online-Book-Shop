using MediatR;

namespace Application.Features.Orders.Commands
{
    public class DeleteOrder : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
