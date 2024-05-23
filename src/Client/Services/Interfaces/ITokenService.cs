using IdentityModel.Client;

namespace Client.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
