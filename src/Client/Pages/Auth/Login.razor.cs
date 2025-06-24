using System.ComponentModel.DataAnnotations;
using Client.Models.Auth;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Pages.Auth
{
    public partial class Login
    {
        private LoginModel loginModel = new();
        private string error = string.Empty;
        private bool isLoading = false;

        [Inject] private IAuthService _authService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                NavigationManager.NavigateTo("/", replace: true);
            }
        }

        private async Task LoginAsync()
        {
            error = string.Empty;
            isLoading = true;

            var result = await _authService.LoginAsync(loginModel.Email, loginModel.Password);

            isLoading = false;

            if (result.IsSuccess)
            {
                NavigationManager.NavigateTo("/", replace: true);
            }
            else
            {
                error = result.ErrorMessage ?? "Login failed.";
            }
        }
    }
}
