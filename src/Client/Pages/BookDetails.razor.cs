using Client.Models;
using Client.Services;
using Client.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cors.Infrastructure;


namespace Client.Pages
{
    public partial class BookDetails
    {
        [Parameter]
        public string bookTitle { get; set; }

        private BookModel Book { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private ITokenService TokenService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IShoppingCartService CartService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var tokenResponse = await TokenService.GetToken("Catalog.read");

            HttpClient.SetBearerToken(tokenResponse.AccessToken);

            var result = await HttpClient.GetAsync(Config["apiUrl"] + $"/catalog/GetBookByTitle/{bookTitle}");

            if (result.IsSuccessStatusCode)
            {
                Book =  await result.Content.ReadFromJsonAsync<BookModel>();
            }

            StateHasChanged();
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
        private void GoBack()
        {
            // Навігація назад
            NavigationManager.NavigateTo("/catalog");
        }
    }
}
