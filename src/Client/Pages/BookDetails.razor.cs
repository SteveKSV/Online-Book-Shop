using Client.Models;
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;


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
        protected override async Task OnInitializedAsync()
        {
            var tokenResponse = await TokenService.GetToken("Catalog.read");

            StateHasChanged();

            HttpClient.SetBearerToken(tokenResponse.AccessToken);

            var result = await HttpClient.GetAsync(Config["apiUrl"] + $"/catalog/GetBookByTitle/{bookTitle}");

            StateHasChanged();

            if (result.IsSuccessStatusCode)
            {
                Book =  await result.Content.ReadFromJsonAsync<BookModel>();
            }

            StateHasChanged();
        }

        private void GoBack()
        {
            // Навігація назад
            NavigationManager.NavigateTo("/catalog");
        }
    }
}
