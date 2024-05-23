using MediatR;

namespace Application.Features.Orders.Commands
{
    public class UpdateUsername : IRequest<bool>
    {
        public string OldUsername { get; set;}
        public string NewUsername { get; set; }

    }
}
