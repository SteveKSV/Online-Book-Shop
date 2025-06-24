namespace Identity.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
    }
}
