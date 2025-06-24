
using Client.Models.Basket;
using Client.Models.Catalog;
using Microsoft.AspNetCore.WebUtilities;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        private string? searchTerm = null;
        private string? searchInput = null;
        private string? sortOrder = "none";
        private string? genresQuery = null;
        private List<string> genres = new();
        private bool IsAuthorized = false;

        private PaginationMetadata pagination = new PaginationMetadata();

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                IsAuthorized = false;
            }
            else
            {
                IsAuthorized = true;
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            int page = 1;
            if (query.TryGetValue("page", out var pageParam) && int.TryParse(pageParam, out int parsedPage))
                page = parsedPage;

            if (query.TryGetValue("title", out var titleParam))
            {
                searchTerm = titleParam.ToString();
                searchInput = searchTerm;
            }

            if (query.TryGetValue("sortOrder", out var sortOrderParam))
                sortOrder = sortOrderParam.ToString();

            if (query.TryGetValue("genre", out var genreParam))
            {
                genresQuery = Uri.UnescapeDataString(genreParam);
            }

            if (!genres.Any())
            {
                genres = (await Service.Get("genre")).OrderBy(g => g).ToList();
            }
            await LoadBooks(page);
        }

        private async Task NavigateToPage(int page)
        {
            if (page != pagination.CurrentPage)
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["page"] = page.ToString(),
                    ["title"] = searchTerm,
                    ["sortOrder"] = sortOrder,
                    ["genre"] = genresQuery
                };

                var uri = QueryHelpers.AddQueryString("/catalog", queryParams);
                Navigation.NavigateTo(uri);
                await LoadBooks(page);
            }
        }
        protected async Task LoadBooks(int page = 1)
        {
            string? encodedGenre = genresQuery;

            // Перевірка: якщо genresQuery не містить '%', то кодуємо
            if (!string.IsNullOrEmpty(genresQuery) && !genresQuery.Contains('%'))
            {
                encodedGenre = Uri.EscapeDataString(genresQuery);
            }

            
            var queryString = $"?pageNumber={page}&pageSize=9&title={searchTerm}&sortOrder={sortOrder}&genre={encodedGenre}";

            var (loadedBooks, loadedPagination) = await Service.GetBooks(queryString);
            if (loadedBooks != null)
            {
                books = loadedBooks;
                pagination = loadedPagination;
            }

            StateHasChanged();
        }

        private async Task UpdateUrlAndReload(int page = 1)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["title"] = searchTerm,
                ["sortOrder"] = sortOrder,
                ["genre"] = genresQuery
            };

            var uri = QueryHelpers.AddQueryString("/catalog", queryParams);
            Navigation.NavigateTo(uri, forceLoad: false);

            await LoadBooks(page);
        }

        private async Task ApplySearch()
        {
            searchTerm = string.IsNullOrWhiteSpace(searchInput) ? null : searchInput;
            await UpdateUrlAndReload(1);
        }

        private async Task ClearSearch()
        {
            searchInput = null;
            searchTerm = null;
            await UpdateUrlAndReload(1);
        }

        private async void SortBooks(string sortOrder)
        {
            this.sortOrder = sortOrder;
            await UpdateUrlAndReload(1);
        }

        private async Task FilterBooksByGenre(string? queryString)
        {
            genresQuery = queryString == "clear" ? null : queryString;
            await UpdateUrlAndReload(1);
        }

        private async Task AddToCart(BookModel book)
        {
            if (!IsAuthorized)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            var item = new ShoppingCartItem
            {
                BookId = book.Id,
                Title = book.Title,
                Price = book.Price,
                Quantity = 1,
                CoverImage = book.CoverImage,
            };

            await CartService.AddOrUpdateItem(item.BookId, item.Quantity, item.Price);
            StateHasChanged();
        }

        private string TruncateTitle(string title, int maxLength)
        {
            return string.IsNullOrEmpty(title) || title.Length <= maxLength ? title : title.Substring(0, maxLength) + "...";
        }
    }
}