namespace Client.Models.Catalog
{
    public class CatalogState
    {
        public int Page { get; set; } = 1;
        public string? Genre { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
    }
}
