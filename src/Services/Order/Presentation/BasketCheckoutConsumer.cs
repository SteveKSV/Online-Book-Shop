using Application.Features.Orders.Commands;
using AutoMapper;
using EventBusMessages.Events;
using MassTransit;
using MediatR;

namespace Order
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger<BasketCheckoutConsumer> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            try
            {
                //var command = _mapper.Map<CheckoutOrder>(context.Message);
                var basketCheckoutEvent = context.Message;
                var command = new CheckoutOrder
                {
                    OrderDto = new Application.Dtos.AddOrderDto
                    {
                        UserName = basketCheckoutEvent.OrderDto.UserName,
                        TotalPrice = basketCheckoutEvent.OrderDto.TotalPrice,
                        Quantity = basketCheckoutEvent.OrderDto.Quantity,
                        FirstName = basketCheckoutEvent.OrderDto.FirstName,
                        LastName = basketCheckoutEvent.OrderDto.LastName,
                        EmailAddress = basketCheckoutEvent.OrderDto.EmailAddress,
                        AddressLine = basketCheckoutEvent.OrderDto.AddressLine,
                        Country = basketCheckoutEvent.OrderDto.Country,
                        State = basketCheckoutEvent.OrderDto.State,
                        ZipCode = basketCheckoutEvent.OrderDto.ZipCode,
                        CardName = basketCheckoutEvent.OrderDto.CardName,
                        CardNumber = basketCheckoutEvent.OrderDto.CardNumber,
                        Expiration = basketCheckoutEvent.OrderDto.Expiration,
                        CVV = basketCheckoutEvent.OrderDto.CVV,
                        PaymentMethod = basketCheckoutEvent.OrderDto.PaymentMethod,
                        Items = basketCheckoutEvent.OrderDto.Items.Select(i => new Application.Dtos.OrderItemDto
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity,
                            Price = i.Price
                        }).ToList()
                    }
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id : {newOrderId}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming BasketCheckoutEvent");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
