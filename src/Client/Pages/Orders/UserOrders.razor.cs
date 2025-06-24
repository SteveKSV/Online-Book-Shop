using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Client.Services.Interfaces;
using Client.Models.Ordering;

namespace Client.Pages.Orders
{
    public partial class UserOrders
    {
        [Inject] protected IOrderService OrderService { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = default!;
        protected List<OrderDTO> Orders { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                                 ?? user.FindFirst("sub")
                                 ?? user.FindFirst("nameid");

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    Orders = await OrderService.GetAllOrdersByUserId(userId);
                }
                else
                {
                    throw new InvalidOperationException("Invalid or missing user ID claim.");
                }
            }
        }
    }
}
