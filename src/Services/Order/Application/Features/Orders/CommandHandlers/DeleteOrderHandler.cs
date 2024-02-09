using Application.Features.Orders.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.CommandHandlers
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrder, bool>
    {
        private readonly IOrderRepository _repository;
        public DeleteOrderHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteOrder request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteOrder(request.Id);
        }
    }
}
