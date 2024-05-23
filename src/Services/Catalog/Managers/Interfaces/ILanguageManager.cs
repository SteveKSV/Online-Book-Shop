namespace Catalog.Managers.Interfaces
{
    public interface ILanguageManager
    {
        Task<IEnumerable<string>> GetAllLanguages();
    }
}
