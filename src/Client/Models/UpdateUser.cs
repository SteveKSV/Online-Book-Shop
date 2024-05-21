namespace Client.Models
{
    public class UpdateUser
    {
        public string NewEmail { get; set; }
        public string OldEmail { get; set; }
        public string OldUsername { get; set; }
        public string NewUsername { get; set; }

    }
}
