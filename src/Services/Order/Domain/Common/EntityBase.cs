namespace Domain.Common
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
