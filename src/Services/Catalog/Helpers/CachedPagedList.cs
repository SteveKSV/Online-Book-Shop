namespace Catalog.Helpers
{
    public class CachedPagedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static CachedPagedList<T> FromPagedList(PagedList<T> paged)
        {
            return new CachedPagedList<T>
            {
                Items = paged.ToList(),
                CurrentPage = paged.CurrentPage,
                TotalPages = paged.TotalPages,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public PagedList<T> ToPagedList()
        {
            return new PagedList<T>(Items, TotalCount, CurrentPage, PageSize);
        }
    }

}
