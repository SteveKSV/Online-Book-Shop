﻿using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;

namespace Shopping.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;
        public BasketService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await _client.GetAsync($"/api/Basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}