namespace Client.Models.Auth
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
