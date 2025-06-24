namespace Client.Models.Auth
{
    public class User
    {
        public Guid UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
