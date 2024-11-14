using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        private string? searchTerm = null;
        private string? sortOrder = null;
        private string? genresQuery = null;
        private PaginationMetadata pagination;

        protected override async Task OnParametersSetAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            int page = 1;
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("page", out var pageParam) && int.TryParse(pageParam, out int parsedPage))
            {
                page = parsedPage;
            }

            await LoadBooks(page);
        }

        private async Task NavigateToPage(int page)
        {
            // Always load the books and update the current page state when a page is clicked
            if (page != pagination.CurrentPage)
            {
                // Navigate only when user clicks on a page number
                Navigation.NavigateTo($"/catalog?page={page}");
                await LoadBooks(page);  // Load books for the clicked page
            }
        }

        protected async Task LoadBooks(int page = 1)
        {
            var queryString = $"?pageNumber={page}&pageSize=9&title={searchTerm}&sortOrder={sortOrder}&genre={genresQuery}";

            var (loadedBooks, loadedPagination) = await Service.GetBooks(queryString);
            if (loadedBooks != null)
            {
                books = loadedBooks;
                pagination = loadedPagination;
            }

            StateHasChanged();
        }

        private async Task SearchBooks(ChangeEventArgs e)
    {
        searchTerm = e.Value.ToString();
        await LoadBooks();
    }

    private async void SortBooks(string sortOrder)
    {
        this.sortOrder = sortOrder;
        await LoadBooks(pagination.CurrentPage);
    }

    private async Task FilterBooksByGenre(string? queryString)
    {
        if (queryString == "clear")
        {
            genresQuery = null;
            sortOrder = null;
        }
        else if (queryString != null)
        {
            genresQuery = queryString;
        }
        else
        {
            genresQuery = null;
        }

        await LoadBooks();
    }

    private async Task AddToCart(BookModel book)
    {
        var item = new ShoppingCartItem
        {
            ProductId = book.Id,
            ProductName = book.Title,
            Price = book.Price,
            Quantity = 1
        };

        await CartService.AddToCart(item);
        StateHasChanged();
    }

    private string TruncateTitle(string title, int maxLength)
    {
        return string.IsNullOrEmpty(title) || title.Length <= maxLength ? title : title.Substring(0, maxLength) + "...";
    }
    }
}
