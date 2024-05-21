using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Orders.CommandHandlers
{
    public class UpdateUsernameHandler : IRequestHandler<UpdateUsername, bool>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        public UpdateUsernameHandler(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateUsername request, CancellationToken cancellationToken)
        {
            return await _repository.UpdateUserNameInOrders(request);
        }
    }
}
