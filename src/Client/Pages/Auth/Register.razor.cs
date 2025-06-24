using Client.Models.Auth;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Client.Pages.Auth
{
    public partial class RegisterBase : ComponentBase
    {
        [Inject] protected IAuthService AuthService { get; set; } = null!;
        [Inject] protected NavigationManager Navigation { get; set; } = null!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        protected RegisterUser RegisterModel = new RegisterUser();
        protected string ErrorMessage = string.Empty;
        protected bool IsLoading = false;
       

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                Navigation.NavigateTo("/", replace: true);
            }
        }
        protected async Task HandleRegister(EditContext editContext)
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var result = await AuthService.RegisterAsync(RegisterModel.Username, RegisterModel.Email, RegisterModel.Password);

            IsLoading = false;

            if (result.IsSuccess)
            {
                Navigation.NavigateTo("/login", forceLoad: true);
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Registration failed.";
            }
        }
    }
}
