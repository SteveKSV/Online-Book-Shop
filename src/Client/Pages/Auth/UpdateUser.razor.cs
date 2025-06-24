using Client.Helpers;
using Client.Models.Auth;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Client.Pages.Auth
{
    public partial class UpdateUser
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected JwtAuthStateProvider AuthProvider { get; set; } = default!;
        [Inject] protected IConfiguration Configuration { get; set; } = default!;
        [Inject] protected IAuthService AuthService { get; set; } = default!;
        [Inject] protected ProtectedLocalStorage _localStorage { get; set; } = default!;
        protected UpdateUserDTO UserModel { get; set; } = new();
        protected string SuccessMessage = string.Empty;
        protected string ErrorMessage = string.Empty;

        protected bool IsLoading = false;

        private string ApiUrl => Configuration["apiUrl"] ?? "https://localhost:5005";
        protected override void OnInitialized()
        {
            var currentUser = AuthProvider.CurrentUser;
            if (currentUser == null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            UserModel.Id = currentUser.UserId;
            UserModel.UserName = currentUser.UserName;
            UserModel.Email = currentUser.Email;
        }

        protected async Task HandleValidSubmit()
        {
            IsLoading = true;
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;

            try
            {
                var tokenResult = await _localStorage.GetAsync<string>("access_token");
                if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
                {
                    ErrorMessage = "User is not authenticated.";
                    return;
                }

                var result = await AuthService.UpdateUserAsync(UserModel, tokenResult.Value);

                if (result.IsSuccess)
                {
                    SuccessMessage = "User updated successfully!";
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Failed to update user.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

    }
}
