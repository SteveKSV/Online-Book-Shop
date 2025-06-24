using Application.Dtos;
using MediatR;

namespace Application.Features.PaymentMethods.Queries
{
    public class GetPaymentMethods : IRequest<List<PaymentMethodDto>>
    {
    }
}
