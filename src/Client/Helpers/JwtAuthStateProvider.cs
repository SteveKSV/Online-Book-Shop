using Client.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Helpers
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly HttpClient _httpClient;
        public User? CurrentUser { get; private set; }

        public JwtAuthStateProvider(ProtectedLocalStorage localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenResult = await _localStorage.GetAsync<string>("access_token");
            var token = tokenResult.Success ? tokenResult.Value : null;

            if (string.IsNullOrWhiteSpace(token))
            {
                CurrentUser = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Перевірка на expiration
            var payload = token.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(AddPadding(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null && keyValuePairs.TryGetValue("exp", out var expObj))
            {
                long expUnix = 0;
                if (expObj is JsonElement expElement && expElement.ValueKind == JsonValueKind.Number && expElement.TryGetInt64(out expUnix))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                    if (expDate < DateTimeOffset.UtcNow)
                    {
                        await _localStorage.DeleteAsync("access_token");
                        CurrentUser = null;
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            CurrentUser = ParseUserFromJwt(token);

            return new AuthenticationState(user);
        }


        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(AddPadding(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? "")) ?? Enumerable.Empty<Claim>();
        }

        private User ParseUserFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(AddPadding(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var user = new User();

            if (keyValuePairs != null)
            {
                if (keyValuePairs.TryGetValue("sub", out var userId))
                {
                    if (userId is JsonElement je && je.ValueKind == JsonValueKind.String)
                    {
                        var userIdString = je.GetString();
                        if (Guid.TryParse(userIdString, out var guid))
                            user.UserId = guid;
                    }
                }

                if (keyValuePairs.TryGetValue("email", out var email))
                    user.Email = email?.ToString() ?? "";

                // Замінив "unique_name" на "username"
                if (keyValuePairs.TryGetValue("username", out var userName))
                    user.UserName = userName?.ToString() ?? "";

                // Поле ролі в токені під довгим URI-ключем
                const string roleClaimKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
                if (keyValuePairs.TryGetValue(roleClaimKey, out var role))
                    user.Role = role?.ToString() ?? "";
            }

            return user;
        }


        public void MarkUserAsAuthenticated(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            CurrentUser = ParseUserFromJwt(token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private string AddPadding(string base64)
        {
            return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        }
    }

}
