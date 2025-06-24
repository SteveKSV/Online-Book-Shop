using Client.Models.Ordering;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class Checkout
    {
        private BasketCheckout BasketCheckout { get; set; } = new();
        private List<PaymentMethod> PaymentMethods { get; set; } = new();

        private Guid CreditCardMethodId => PaymentMethods.FirstOrDefault(pm => pm.Name == "Credit Card")?.Id ?? Guid.Empty;

        private bool IsCreditCardSelected => BasketCheckout.PaymentMethodId == CreditCardMethodId;

        private bool ShowCardValidationErrors = false;

        protected override async Task OnInitializedAsync()
        {
            PaymentMethods = await OrderService.GetPaymentMethods();
            await InitializeUserAsync();
        }

        private async Task InitializeUserAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                string? rawUserId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                    ?? user.FindFirst("sub")?.Value
                                    ?? user.FindFirst("nameid")?.Value;

                if (rawUserId != null && Guid.TryParse(rawUserId, out var parsedUserId))
                {
                    BasketCheckout.UserId = parsedUserId;
                }
                else
                {
                    throw new InvalidOperationException("UserId claim is missing or invalid.");
                }
            }
            else
            {
                throw new InvalidOperationException("User is not authenticated.");
            }
        }

        private void OnPaymentMethodChanged(ChangeEventArgs e)
        {
            if (Guid.TryParse(e.Value?.ToString(), out var selectedId))
            {
                BasketCheckout.PaymentMethodId = selectedId;
                ShowCardValidationErrors = false;
            }
        }

        private async Task HandleValidSubmit()
        {
            ShowCardValidationErrors = false;

            if (IsCreditCardSelected)
            {
                if (string.IsNullOrWhiteSpace(BasketCheckout.CardNumber) ||
                    string.IsNullOrWhiteSpace(BasketCheckout.Expiration) ||
                    string.IsNullOrWhiteSpace(BasketCheckout.CVV))
                {
                    ShowCardValidationErrors = true;
                    return;
                }
            }

            // UserId вже ініціалізований у InitializeUserAsync
            await OrderService.PlaceOrder(BasketCheckout);
            NavigationManager.NavigateTo("/orders");
        }
    }
}
