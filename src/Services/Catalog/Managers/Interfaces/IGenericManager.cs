namespace Catalog.Managers.Interfaces
{
    public interface IGenericManager<T> where T : class
    {
        Task<bool> DeleteEntity(Guid id);
    }
}
