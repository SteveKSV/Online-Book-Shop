namespace Catalog.Managers.Interfaces
{
    public interface IGenericManager<T> where T : class
    {
        Task CreateEntity(T entity);
        Task<bool> UpdateEntity(T entity);
        Task<bool> DeleteEntity(string id);
    }
}
