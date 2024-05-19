using Application.Dtos;
using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using AutoMapper;
using Domain.Entities;
using EventBusMessages.Events;

namespace Application.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId)).ReverseMap();

            CreateMap<UpdateOrderDto, Order>()
                   .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<UpdateOrder, Order>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Order.UserName))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Order.TotalPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Order.Quantity))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Order.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Order.LastName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Order.EmailAddress))
                .ForMember(dest => dest.AddressLine, opt => opt.MapFrom(src => src.Order.AddressLine))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Order.Country))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Order.State))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Order.ZipCode))
                .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.Order.CardName))
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.Order.CardNumber))
                .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Order.Expiration))
                .ForMember(dest => dest.CVV, opt => opt.MapFrom(src => src.Order.CVV))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Order.PaymentMethod))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Order.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            CreateMap<BasketCheckoutEvent, CheckoutOrder>().ReverseMap();

            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderDto, GetOrderById>().ReverseMap();
            CreateMap<OrderDto, GetAllOrders>().ReverseMap();

            CreateMap<CheckoutOrder, Order>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.OrderDto.UserName))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderDto.TotalPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.OrderDto.Quantity))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.OrderDto.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.OrderDto.LastName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.OrderDto.EmailAddress))
                .ForMember(dest => dest.AddressLine, opt => opt.MapFrom(src => src.OrderDto.AddressLine))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.OrderDto.Country))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.OrderDto.State))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.OrderDto.ZipCode))
                .ForMember(dest => dest.CardName, opt => opt.MapFrom(src => src.OrderDto.CardName))
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.OrderDto.CardNumber))
                .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.OrderDto.Expiration))
                .ForMember(dest => dest.CVV, opt => opt.MapFrom(src => src.OrderDto.CVV))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.OrderDto.PaymentMethod))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            CreateMap<AddOrderDto, Order>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));
        }
    }
}
