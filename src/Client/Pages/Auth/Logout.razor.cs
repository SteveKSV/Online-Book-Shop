using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;


namespace Client.Pages.Auth
{
    public partial class Logout
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthService.Logout();
            Navigation.NavigateTo("/", forceLoad: true);
        }
    }
}
