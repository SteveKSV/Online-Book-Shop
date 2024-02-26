using Basket.API.GrpcServices;
using Basket.Entities;
using Basket.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketManager _repository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketManager repository, DiscountGrpcService discountGrpcService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService)); ;
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
    }
}
