using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Order.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllOrdersAsync()
        {
            var orders = await _mediator.Send(new GetAllOrders());
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrderByIdAsync(int id)
        {
            var order = await _mediator.Send(new GetOrderById() { Id = id });
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> CheckoutOrderAsync([FromBody] CheckoutOrder order)
        {
            var createdOrder = await _mediator.Send(order);
            return Ok(createdOrder);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateOrderAsync([FromBody] UpdateOrder order)
        {
            if (await _mediator.Send(order))
            {
                return Ok(order);
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderAsync(int id)
        {
            if (await _mediator.Send(new DeleteOrder() { Id = id}))
            {
                return Ok($"Order with Id {id} was successfully deleted");
            }

            return BadRequest($"Order with Id {id} wasn't deleted");
        }
    }
}
