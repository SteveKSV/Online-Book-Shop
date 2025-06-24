using Identity.Helpers.Catalog.Helpers;
using Identity.Models;

namespace Identity.Managers.Interfaces
{
    public interface IAuthManager
    {
        Task<AuthResponse> SignIn(LoginDTO loginInfo);
        Task<AuthResponse> SignUp(RegisterDTO registerInfo);
        Task<PagedList<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<UserDTO?> GetUserByIdAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<AuthResponse> UpdateUserAsync(UpdateUserDTO userDto);

    }
}
