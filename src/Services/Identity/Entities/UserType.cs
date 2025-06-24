namespace Identity.Entities
{
    public class UserType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
