using Client.Models;
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        private List<BookModel> _filteredBooks = new();
        private List<string> Genres { get; set; } = new List<string>();
        private List<string> Languages { get; set; } = new List<string>();
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
                _filteredBooks = books.ToList();
                Genres = books.SelectMany(book => book.Genre.Split(',')).Distinct().ToList();
                Languages = books.SelectMany(book => book.LanguageName.Split(',')).Distinct().ToList();
            }

            StateHasChanged();
        }

        private void UpdateFilteredBooksSearchBar(string searchTerm)
        {

            if (string.IsNullOrEmpty(searchTerm))
            {
                _filteredBooks = books.ToList();
            }
            else
            {
                _filteredBooks = books.Where(prop =>
                            prop.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            prop.AuthorName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            prop.PublisherName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            prop.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                            ).ToList();
            }
        }

        private void SortBooks(string sortOrder)
        {
            if (sortOrder == "asc")
            {
                _filteredBooks = _filteredBooks.OrderBy(book => book.Price).ToList();
            }
            else if (sortOrder == "desc")
            {
                _filteredBooks = _filteredBooks.OrderByDescending(book => book.Price).ToList();
            }
        }

        private void FilterBooksByGenre(List<string> selectedGenres)
        {
            if (selectedGenres == null || selectedGenres.Count == 0)
            {
                
                _filteredBooks = books.ToList();
            }
            else
            {
                _filteredBooks = books.Where(book => selectedGenres.All(genre => book.Genre.Contains(genre))).ToList();

                if (_filteredBooks.Count == 0)
                {
                    _filteredBooks = new List<BookModel>();
                }
            }
        }

        private void FilterBooksByLanguage(List<string> selectedLanguages)
        {
            if (selectedLanguages == null || selectedLanguages.Count == 0)
            {

                _filteredBooks = books.ToList();
            }
            else
            {
                _filteredBooks = books.Where(book => selectedLanguages.All(language => book.LanguageName.Contains(language))).ToList();

                if (_filteredBooks.Count == 0)
                {
                    _filteredBooks = new List<BookModel>();
                }
            }
        }
    }
}
