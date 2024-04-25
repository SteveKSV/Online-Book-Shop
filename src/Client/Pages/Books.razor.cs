using Client.Models;
using Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Client.Pages
{
    public partial class Books
    {
        private List<BookModel> books = new();
        private List<BookModel> _filteredBooks = new();
        private int TotalBooksCount { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private IConfiguration Config { get; set; }
        //[Inject] private ITokenService TokenService { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadBooks();
            await GetTotalBooksCount();
        }

        private int CalculateTotalPages()
        {
            int pageSize = 3;
            int totalPages = (int)Math.Ceiling((double)TotalBooksCount / pageSize);
            return totalPages;
        }
        protected async Task GetTotalBooksCount()
        {
            try
            {
                var apiUrl = Config["apiUrl"];
                var result = await HttpClient.GetAsync($"{apiUrl}/catalog/GetBooksCount");

                if (result.IsSuccessStatusCode)
                {
                    TotalBooksCount = await result.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log or show error message
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        protected async Task LoadBooks(string queryString = null)
        {
            try
            {
                var apiUrl = Config["apiUrl"];
                var result = await HttpClient.GetAsync($"{apiUrl}/catalog{queryString}");

                if (result.IsSuccessStatusCode)
                {
                    books = await result.Content.ReadFromJsonAsync<List<BookModel>>();
                    _filteredBooks = books.ToList();
                    StateHasChanged(); 
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log or show error message
                Console.WriteLine($"Error: {ex.Message}");
            }
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
