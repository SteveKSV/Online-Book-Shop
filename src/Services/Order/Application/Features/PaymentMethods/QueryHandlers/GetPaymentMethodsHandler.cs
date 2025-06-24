using Application.Dtos;
using Application.Features.PaymentMethods.Queries;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.PaymentMethods.QueryHandlers
{
    public class GetPaymentMethodsHandler : IRequestHandler<GetPaymentMethods, List<PaymentMethodDto>>
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IMapper _mapper;

        public GetPaymentMethodsHandler(IPaymentMethodRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PaymentMethodDto>> Handle(GetPaymentMethods request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetPaymentMethods();
            return _mapper.Map<List<PaymentMethodDto>>(orders);
        }
    }
}
