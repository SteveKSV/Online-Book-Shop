using Client.Models;
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        [Inject] private ITokenService TokenService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var tokenResponse = await TokenService.GetToken("Catalog.read");
            StateHasChanged();
            HttpClient.SetBearerToken(tokenResponse.AccessToken);
            var result = await HttpClient.GetAsync(Config["apiUrl"] + "/catalog");
            StateHasChanged();
            if (result.IsSuccessStatusCode)
            {
                books = await result.Content.ReadFromJsonAsync<List<BookModel>>();
            }
            StateHasChanged();
        }
    }
}
