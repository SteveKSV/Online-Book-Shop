using Application.Dtos;
using Application.Features.Orders.Commands;
using AutoMapper;
using EventBusMessages.Events;
using MassTransit;
using MediatR;
using System.Globalization;

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
                var basketCheckout = context.Message;

                int expiryMonth = 0;
                int expiryYear = 0;

                var formats = new[] { "MM/yy", "MM/yyyy" };
                if (DateTime.TryParseExact(basketCheckout.Expiration, formats,
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var parsedDate))
                {
                    expiryMonth = parsedDate.Month;
                    expiryYear = parsedDate.Year;
                }

                CardPaymentDto? cardPaymentDto = null;

                if (!string.IsNullOrEmpty(basketCheckout.CardNumber)
                    && expiryMonth > 0
                    && expiryYear > 0
                    && !string.IsNullOrEmpty(basketCheckout.CVV))
                {
                    cardPaymentDto = new CardPaymentDto
                    {
                        CardNumber = basketCheckout.CardNumber,
                        ExpiryMonth = expiryMonth,
                        ExpiryYear = expiryYear,
                        Cvv = basketCheckout.CVV
                    };
                }

                var command = new CheckoutOrder
                {
                    OrderDto = new AddOrderDto
                    {
                        UserId = basketCheckout.UserId,
                        TotalPrice = basketCheckout.TotalPrice,
                        Quantity = basketCheckout.Quantity,

                        FirstName = basketCheckout.FirstName,
                        LastName = basketCheckout.LastName,
                        EmailAddress = basketCheckout.EmailAddress,
                        Address = basketCheckout.Address,

                        StatusId = Guid.Parse("F23B2522-7D48-F011-AFA4-38D57A8AEC11"),

                        Payment = new PaymentDto
                        {
                            PaymentMethodId = basketCheckout.PaymentMethodId,
                            Amount = basketCheckout.TotalPrice,
                            CardPayment = cardPaymentDto
                        },

                        Items = basketCheckout.Items.Select(i => new OrderItemDto
                        {
                            BookId = i.BookId,
                            Quantity = i.Quantity,
                            Price = i.Price
                        }).ToList(),

                    }
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id: {newOrderId}", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming BasketCheckoutEvent");
            }
        }

    }
}
