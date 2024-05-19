using EventBusMessages.Common;

namespace EventBusMessages.Events
{
    public class BasketCheckoutEvent
    {
        public Order OrderDto { get; set; }
    }
}
