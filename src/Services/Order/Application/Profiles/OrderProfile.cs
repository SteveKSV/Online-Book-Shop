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
            // 🔁 OrderItem
            CreateMap<OrderItem, OrderItemDto>()
             .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
             .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
             .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Authors));

            // 🔁 PaymentMethod
            CreateMap<PaymentMethod, PaymentMethodDto>().ReverseMap();

            // Payment -> PaymentDto
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.CardPayment, opt => opt.MapFrom(src => src.CardPayments.FirstOrDefault())); 

            // PaymentDto -> Payment
            CreateMap<PaymentDto, Payment>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
                .ForMember(dest => dest.CardPayments, opt => opt.MapFrom(src => src.CardPayment != null ? new List<CardPayment> {
                    new CardPayment {
                        CardNumber = src.CardPayment.CardNumber,
                        Cvv = src.CardPayment.Cvv,
                        ExpiryMonth = src.CardPayment.ExpiryMonth,
                        ExpiryYear = src.CardPayment.ExpiryYear
                    }
                            } : new List<CardPayment>()));

            CreateMap<CardPayment, CardPaymentDto>().ReverseMap();

            // 🔁 Order <-> OrderDto
            CreateMap<Order, OrderDto>()
             .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
             .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name)) 
             .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
             .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // 🔁 AddOrderDto -> Order
            CreateMap<AddOrderDto, Order>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            // 🔁 Order -> AddOrderDto
            CreateMap<Order, AddOrderDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemDto
                {
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            // 🔁 UpdateOrderDto -> Order
            CreateMap<UpdateOrderDto, Order>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            // 🔁 UpdateOrder -> Order
            CreateMap<UpdateOrder, Order>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Order.UserId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Order.TotalPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Order.Quantity))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Order.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Order.LastName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Order.EmailAddress))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Order.Address))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Order.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Order.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Order.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            // 🔁 BasketCheckoutEvent -> CheckoutOrder
            CreateMap<BasketCheckoutEvent, CheckoutOrder>()
                .ForMember(dest => dest.OrderDto, opt => opt.MapFrom(src => src)); // весь src у OrderDto

            // BasketCheckoutEvent → AddOrderDto
            CreateMap<BasketCheckoutEvent, AddOrderDto>()
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => new PaymentDto
                {
                    Amount = src.TotalPrice,
                    PaymentMethodId = src.PaymentMethodId
                }))
                .ForMember(dest => dest.StatusId, opt => opt.Ignore()) // буде встановлено окремо
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // OrderItem → OrderItemDto
            CreateMap<OrderItem, OrderItemDto>();

            // 🔁 CheckoutOrder -> Order
            CreateMap<CheckoutOrder, Order>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.OrderDto.UserId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderDto.TotalPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.OrderDto.Quantity))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.OrderDto.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.OrderDto.LastName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.OrderDto.EmailAddress))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.OrderDto.Address))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.OrderDto.StatusId))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.OrderDto.Payment))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BookId = i.BookId,
                    Quantity = i.Quantity,
                    Price = i.Price
                })));

            // 🔁 Query DTOs
            CreateMap<OrderDto, GetOrderById>().ReverseMap();
            CreateMap<OrderDto, GetAllOrders>().ReverseMap();
        }
    }
}
