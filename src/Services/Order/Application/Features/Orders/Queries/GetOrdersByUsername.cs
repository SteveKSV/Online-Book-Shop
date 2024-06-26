﻿using Application.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Orders.Queries
{
    public class GetOrdersByUsername : IRequest<List<OrderDto>>
    {
        public string UserName { get; set; }

        public GetOrdersByUsername(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
