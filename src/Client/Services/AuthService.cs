using Client.Helpers;
using Client.Models.Auth;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ProtectedLocalStorage _localStorage;
        private readonly JwtAuthStateProvider _authStateProvider;
        private readonly string _apiUrl;

        public AuthService(HttpClient httpClient, ProtectedLocalStorage localStorage, JwtAuthStateProvider authStateProvider, IConfiguration configuration)
        {
            _http = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _apiUrl = configuration["apiUrl"] ?? throw new ArgumentNullException("apiUrl is not configured.");
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync($"{_apiUrl}/auth/signin", new { Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = errorResponse?.StatusMessage ?? "Login failed"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                return new AuthResult { IsSuccess = false, ErrorMessage = "No token received." };
            }

            await _localStorage.SetAsync("access_token", result.Token);
            _authStateProvider.MarkUserAsAuthenticated(result.Token);

            return new AuthResult { IsSuccess = true };
        }

        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            var response = await _http.PostAsJsonAsync($"{_apiUrl}/auth/signup", new
            {
                UserName = username,
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = errorResponse?.StatusMessage ?? "Registration failed"
                };
            }

            return new AuthResult { IsSuccess = true };
        }

        public async Task<AuthResult> UpdateUserAsync(UpdateUserDTO userModel, string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.PutAsJsonAsync($"{_apiUrl}/auth/updateUser", userModel);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = errorResponse?.StatusMessage ?? "Failed to update user"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (!string.IsNullOrWhiteSpace(result?.Token))
            {
                await _localStorage.SetAsync("access_token", result.Token);
                _authStateProvider.MarkUserAsAuthenticated(result.Token);
            }

            return new AuthResult { IsSuccess = true };
        }



        public async Task Logout()
        {
            await _localStorage.DeleteAsync("access_token");
        }
    }
}
