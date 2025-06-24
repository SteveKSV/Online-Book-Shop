using Client.Models.Auth;

namespace Client.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RegisterAsync(string username, string email, string password);
        Task<AuthResult> UpdateUserAsync(UpdateUserDTO userModel, string token);
        Task Logout();
    }
}
