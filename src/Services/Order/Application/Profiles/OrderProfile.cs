using Application.Features.Orders.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, CheckoutOrder>().ReverseMap();
            CreateMap<Order, UpdateOrder>().ReverseMap();
        }
    }
}
