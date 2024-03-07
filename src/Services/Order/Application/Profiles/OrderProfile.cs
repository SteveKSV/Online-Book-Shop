using Application.Features.Orders.Commands;
using AutoMapper;
using Domain.Entities;
using EventBusMessages.Events;

namespace Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, CheckoutOrder>().ReverseMap();
            CreateMap<Order, UpdateOrder>().ReverseMap();
            CreateMap<BasketCheckoutEvent, CheckoutOrder>().ReverseMap();
        }
    }
}
