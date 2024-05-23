    using Basket.API.GrpcServices;
    using Basket.Entities;
    using Basket.Managers.Interfaces;
using EventBusMessages.Common;
using EventBusMessages.Events;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;
    using System.Reflection.Emit;

    namespace Basket.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BasketController : ControllerBase
        {
            private readonly IBasketManager _repository;
            private readonly DiscountGrpcService _discountGrpcService;
            private readonly IPublishEndpoint _publishEndpoint;
            public BasketController(IBasketManager repository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint = null)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
                _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(discountGrpcService));
            }

            [HttpGet("{userName}")]
            [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
            public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
            {
                var basket = await _repository.GetBasket(userName);
                return Ok(basket ?? new ShoppingCart(userName));
            }
            [HttpPost]
            [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
            public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
            {
                try
                {
                    // Communicate with Discount.Grpc and calculate latest prices of products into sc
                    foreach (var item in basket.Items)
                    {
                        try
                        {
                            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                            item.Price -= coupon.Amount;
                        }
                        catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.Unavailable)
                        {
                            // Log the error or perform any necessary actions
                            // For now, we'll just ignore the error and continue with the loop
                        }
                    }

                    return Ok(await _repository.UpdateBasket(basket));
                }
                catch (Exception ex)
                {
                    // Handle other exceptions if needed
                    // Log the error or perform any necessary actions
                    return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the basket.");
                }
            }

            [HttpDelete("{userName}")]
            [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
            public async Task<IActionResult> DeleteBasket(string userName)
            {
                await _repository.DeleteBasket(userName);
                return Ok();
            }

            [Route("[action]")]
            [HttpPost]
            [ProducesResponseType((int)HttpStatusCode.Accepted)]
            [ProducesResponseType((int)HttpStatusCode.BadRequest)]
            public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
            {
                // get existing basket with total price            
                // Set TotalPrice on basketCheckout eventMessage
                // send checkout event to rabbitmq
                // remove the basket

                // get existing basket with total price
                var basket = await _repository.GetBasket(basketCheckout.UserName);
                if (basket == null)
                {
                    return BadRequest();
                }

                // send checkout event to rabbitmq
                var orderDto = new Order
                {
                    UserName = basket.UserName,
                    TotalPrice = basket.TotalPrice,
                    Quantity = basketCheckout.Quantity,
                    FirstName = basketCheckout.FirstName,
                    LastName = basketCheckout.LastName,
                    EmailAddress = basketCheckout.EmailAddress,
                    AddressLine = basketCheckout.AddressLine,
                    Country = basketCheckout.Country,
                    State = basketCheckout.State,
                    ZipCode = basketCheckout.ZipCode,
                    CardName = basketCheckout.CardName,
                    CardNumber = basketCheckout.CardNumber,
                    Expiration = basketCheckout.Expiration,
                    CVV = basketCheckout.CVV,
                    PaymentMethod = basketCheckout.PaymentMethod,
                    Items = basket.Items.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                };

                var eventMessage = new BasketCheckoutEvent
                {
                    OrderDto = orderDto
                };

                await _publishEndpoint.Publish<BasketCheckoutEvent>(eventMessage);

                // remove the basket
                await _repository.DeleteBasket(basket.UserName);

                return Accepted();
            }

            [HttpDelete("{userName}/items/{productId}")]
            [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
            public async Task<ActionResult<ShoppingCart>> RemoveItem(string userName, string productId)
            {
                var basket = await _repository.RemoveItemFromBasket(userName, productId);
                if (basket == null)
                {
                    return NotFound();
                }

                return Ok(basket);
            }

        [HttpPut("{userName}/items/{productId}/quantity/{quantity}")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateItemQuantity(string userName, string productId, int quantity)
        {
            var basket = await _repository.UpdateItemQuantity(userName, productId, quantity);
            if (basket == null)
            {
                return NotFound();
            }

            return Ok(basket);
        }

        [HttpPut("{oldUserName}/update-username/{newUserName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserNameInBasket(string oldUserName, string newUserName)
        {
            await _repository.UpdateUserNameInBasket(oldUserName, newUserName);
            return Ok();
        }
    }
}
