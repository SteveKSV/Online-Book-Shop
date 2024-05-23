using Catalog.Entities;

namespace Catalog.Managers.Interfaces
{
    public interface IGenreManager
    {
        Task<IEnumerable<string>> GetAllGenres();
    }
}
