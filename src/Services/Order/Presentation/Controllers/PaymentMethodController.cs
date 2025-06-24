using Application.Features.PaymentMethods.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Order.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPaymentMethodsAsync()
        {
            var paymentMethods = await _mediator.Send(new GetPaymentMethods());
            return Ok(paymentMethods);
        }
    }
}
