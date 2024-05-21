using Client.Models;

namespace Client.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task ChangeUser(UpdateUser updateUsername);
        Task<string> GetUserEmail(string username);
    }
}
