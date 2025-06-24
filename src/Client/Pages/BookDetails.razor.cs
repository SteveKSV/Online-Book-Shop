using Client.Models.Basket;
using Client.Models.Catalog;
using Client.Models.Catalog.Ratings;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Client.Pages
{
    public partial class BookDetails
    {
        [Parameter]
        public Guid BookId { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        private BookModel Book { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IShoppingCartService CartService { get; set; }
        [Inject] private IRatingService RatingService { get; set; }
        [Inject] private ProtectedLocalStorage LocalStorage { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private int SelectedRating { get; set; } = 0;
        private string RatingStatus { get; set; }
        private bool IsLoggedIn { get; set; }
        private Guid UserId { get; set; }

        protected override async Task OnInitializedAsync()
        {

            var result = await HttpClient.GetAsync(Config["apiUrl"] + $"/catalog/{BookId}");

            if (result.IsSuccessStatusCode)
            {
                Book = await result.Content.ReadFromJsonAsync<BookModel>();
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                string? rawUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? user.FindFirst("sub")?.Value
                                    ?? user.FindFirst("nameid")?.Value;

                if (rawUserId != null && Guid.TryParse(rawUserId, out var parsedUserId))
                {
                    UserId = parsedUserId;
                    IsLoggedIn = true;

                    var userRating = await RatingService.GetUserRatingAsync(BookId, UserId);
                    if (userRating.HasValue)
                    {
                        SelectedRating = userRating.Value;
                    }
                }
                else
                {
                    // Логуй, якщо потрібно, але не кидай
                    IsLoggedIn = false;
                }
            }
            else
            {
                // Просто позначаємо, що користувач не залогінений
                IsLoggedIn = false;
            }


            if (IsLoggedIn)
            {
                var userRating = await RatingService.GetUserRatingAsync(BookId, UserId);
                if (userRating.HasValue)
                {
                    SelectedRating = userRating.Value;
                }
            }
            StateHasChanged();
        }

        private async Task Rate(int rating)
        {
            if (rating < 1 || rating > 5)
            {
                RatingStatus = "Invalid rating value.";
                return;
            }

            SelectedRating = rating;

            var entity = new RateBook
            {
                BookId = Book.Id,
                UserId = UserId,
                Rating = rating
            };

            var success = await RatingService.RateBookAsync(entity);
            if (success)
            {
                RatingStatus = $"You rated this book {rating} stars!";
            }
            else
            {
                RatingStatus = "Failed to update rating.";
            }

            StateHasChanged();
        }

        private async Task AddToCart(BookModel book)
        {
            if (!IsLoggedIn)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            var item = new ShoppingCartItem
            {
                BookId = book.Id,
                Title = book.Title,
                Price = book.Price,
                Quantity = 1
            };

            await CartService.AddOrUpdateItem(item.BookId, item.Quantity, item.Price);
            StateHasChanged();
        }
        private void GoBack()
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                NavigationManager.NavigateTo(ReturnUrl);
            }
            else
            {
                NavigationManager.NavigateTo("/catalog");
            }
        }

        private RenderFragment RenderStars(double rating) => builder =>
        {
            int seq = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (rating >= i)
                {
                    builder.OpenElement(seq++, "i");
                    builder.AddAttribute(seq++, "class", "bi bi-star-fill text-warning me-1");
                    builder.CloseElement();
                }
                else if (rating >= i - 0.5)
                {
                    builder.OpenElement(seq++, "i");
                    builder.AddAttribute(seq++, "class", "bi bi-star-half text-warning me-1");
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(seq++, "i");
                    builder.AddAttribute(seq++, "class", "bi bi-star text-warning me-1");
                    builder.CloseElement();
                }

            }
        };
    }
}
