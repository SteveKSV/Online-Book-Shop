using Client.Models;
using Client.Services;
using Client.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        private string? searchTerm = null;
        private string? sortOrder = null;
        private string? genresQuery = null;
        private string? languagesQuery = null;
        private PaginationMetadata pagination;

        [Inject] private ICatalogService Service { get; set; }
        [Inject] private IShoppingCartService CartService { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await LoadBooks();
        }
        protected async Task LoadBooks(int page = 1)
        {
            var queryString = $"?pageNumber={page}&pageSize=3&title={searchTerm}&sortOrder={sortOrder}{genresQuery}{languagesQuery}";

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
            if (queryString != null)
            {
                genresQuery = queryString;
                
            } 
            else
            {
                genresQuery = null;
            }

            await LoadBooks();
        }

        private async Task FilterBooksByLanguage(string? queryString)
        {
            if (queryString != null)
            {
                languagesQuery = queryString;

            }
            else
            {
                languagesQuery = null;
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
    }
}
