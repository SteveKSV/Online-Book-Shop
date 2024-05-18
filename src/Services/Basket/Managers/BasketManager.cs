using Basket.API.GrpcServices;
using Basket.Entities;
using Basket.Managers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Managers
{
    public class BasketManager : IBasketManager
    {
        private readonly IDistributedCache _redisCache;
        public BasketManager(IDistributedCache cache)
        {
            _redisCache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName);
            if (String.IsNullOrEmpty(basket))
                return null;
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        public async Task<ShoppingCart> UpdateItemQuantity(string userName, string productId, int quantity)
        {
            var basket = await GetBasket(userName);
            if (basket == null) return null;

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return basket;

            item.Quantity = quantity;

            return await UpdateBasket(basket);
        }
        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }

        public async Task<ShoppingCart> RemoveItemFromBasket(string userName, string productId)
        {
            var basket = await GetBasket(userName);
            if (basket == null)
            {
                return null;
            }

            var itemToRemove = basket.Items.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                basket.Items.Remove(itemToRemove);
                await UpdateBasket(basket);
            }

            return basket;
        }
    }
}
