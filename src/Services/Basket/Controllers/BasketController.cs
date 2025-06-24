using Basket.DTO;
using Basket.Entities;
using Basket.Managers.Interfaces;
using EventBusMessages.Common;
using EventBusMessages.Events;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketManager _basketManager;
        private readonly IPublishEndpoint _publishEndpoint;
        public BasketController(IBasketManager basketManager, IPublishEndpoint publishEndpoint)
        {
            _basketManager = basketManager ?? throw new ArgumentNullException(nameof(basketManager));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(BasketDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketDTO>> GetBasket(Guid userId)
        {
            var basket = await _basketManager.GetBasket(userId);
            return Ok(basket);
        }

        [HttpGet("{userId}/count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> GetBasketItemCount(Guid userId)
        {
            var basket = await _basketManager.GetBasket(userId);
            if (basket == null)
            {
                return 0;
            }

            var count = basket.Items.Count();
            return Ok(count);
        }

        [HttpPost("{userId:guid}/items")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketItem>> AddOrUpdateItem(Guid userId, [FromBody] BasketItemDTO itemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _basketManager.AddOrUpdateItem(userId, itemDto.BookId, itemDto.Quantity, itemDto.Price);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to add or update item.");
            }
        }

        [HttpPut("{userId:guid}/items/{bookId:guid}/quantity/{quantity:int}")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketItem>> UpdateItemQuantity(Guid userId, Guid bookId, int quantity)
        {
            var item = await _basketManager.UpdateItemQuantity(userId, bookId, quantity);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(Guid userId)
        {
            await _basketManager.DeleteBasket(userId);
            return Ok();
        }

        [HttpDelete("{userId:guid}/items/{bookId:guid}")]
        [ProducesResponseType(typeof(List<BasketItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<BasketItem>>> RemoveItem(Guid userId, Guid bookId)
        {
            var updatedBasket = await _basketManager.RemoveItemFromBasket(userId, bookId);
            return Ok(updatedBasket);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            if (basketCheckout == null)
                return BadRequest("Invalid checkout request.");

            // Отримати кошик користувача
            var basket = await _basketManager.GetBasket(basketCheckout.UserId);
            if (basket == null || !basket.Items.Any())
                return BadRequest("Basket is empty");

            var totalPrice = basket.Items.Sum(i => i.Price * i.Quantity);
            var totalQuantity = basket.Items.Sum(i => i.Quantity);

            var checkoutEvent = new BasketCheckoutEvent
            {
                UserId = basketCheckout.UserId,
                FirstName = basketCheckout.FirstName,
                LastName = basketCheckout.LastName,
                EmailAddress = basketCheckout.EmailAddress,
                Address = basketCheckout.Address,
                PaymentMethodId = basketCheckout.PaymentMethodId,
                TotalPrice = totalPrice,
                Quantity = totalQuantity,
                Items = basket.Items.Select(i => new OrderItem
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                }).ToList(),

                CardNumber = basketCheckout.CardNumber,
                Expiration = basketCheckout.Expiration,
                CVV = basketCheckout.CVV
            };

            // Відправити подію до RabbitMQ
            await _publishEndpoint.Publish(checkoutEvent);

            // Очистити кошик
            await _basketManager.DeleteBasket(basketCheckout.UserId);

            return Accepted();
        }

    }
}
