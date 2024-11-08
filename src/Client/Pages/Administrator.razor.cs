using Client.Models.ModelsForPrediction;
using System.Text.Json;

namespace Client.Pages
{
    public partial class Administrator
    {

        private List<BookResult> Books { get; set; } = new List<BookResult>();
        private BookResult CurrentBook { get; set; }
        private List<GenrePrediction> Predictions { get; set; } = new List<GenrePrediction>();
        private int CurrentBookIndex { get; set; } = 0;
        private string StatusMessage { get; set; }
        private string ErrorMessage { get; set; }
        private List<ReviewedBook> ReviewedBooks { get; set; } = new List<ReviewedBook>();
        private int SkipCount { get; set; } = 0;
        private bool IsFirstBook => CurrentBookIndex == 0;
        private bool IsLastBook => CurrentBookIndex == Books.Count - 1;
        private bool IsSubmitButtonDisabled => ReviewedBooks.Count < 50;

        // Load the initial set of books with predictions when the component initializes
        protected override async Task OnInitializedAsync()
        {
            await LoadBooksWithPredictions();
        }

        /// <summary>
        /// Fetches books with genre predictions from the API.
        /// If the API is unreachable, sets an error message.
        /// </summary>
        private async Task LoadBooksWithPredictions()
        {
            try
            {
                var response = await Http.GetAsync($"{_configuration.GetSection("flaskApi").Value}/get_books_with_predictions?skip={SkipCount}&limit=70");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(jsonResponse);

                    if (responseObject?.Books != null)
                    { 

                        Books.AddRange(responseObject.Books);
                        Books.OrderByDescending(book => book.Uncertainty);
                        SkipCount += 70;
                        SetCurrentBook();
                    }
                    else
                    {
                        ErrorMessage = "No books found in the response.";
                    }
                }
                else
                {
                    ErrorMessage = "Failed to fetch books from API.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Could not connect to the API to fetch books. Please check the connection or try again later.";
                Console.Error.WriteLine($"Error fetching books: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the current book based on CurrentBookIndex and loads its predictions.
        /// </summary>
        private void SetCurrentBook()
        {
            CurrentBook = Books.ElementAtOrDefault(CurrentBookIndex)!;
            Predictions = CurrentBook?.Predictions ?? new List<GenrePrediction>();
        }

        /// <summary>
        /// Selects a genre for the current book and adds it to the ReviewedBooks list.
        /// Proceeds to the next book if less than 50 books have been reviewed.
        /// </summary>
        private async Task SelectGenre(string selectedGenre)
        {
            if (CurrentBook == null)
            {
                ErrorMessage = "No book selected.";
                return;
            }

            var reviewedBook = ReviewedBooks.FirstOrDefault(b => b.BookId == CurrentBook.Id);
            if (reviewedBook == null)
            {
                reviewedBook = new ReviewedBook
                {
                    BookId = CurrentBook.Id,
                    Title = CurrentBook.Title,
                    SelectedGenre = selectedGenre
                };
                ReviewedBooks.Add(reviewedBook);
            }
            else
            {
                reviewedBook.SelectedGenre = selectedGenre;
            }

            StatusMessage = $"Genre '{selectedGenre}' selected for '{CurrentBook.Title}'.";
            NextBook();
        }

        /// <summary>
        /// Unselects the genre for the current book if it was previously selected,
        /// and removes it from the labeled books list if the genre is unselected.
        /// </summary>
        private async Task UnselectGenre(string selectedGenre)
        {
            if (CurrentBook == null)
            {
                ErrorMessage = "No book selected.";
                return;
            }

            var reviewedBook = ReviewedBooks.FirstOrDefault(b => b.BookId == CurrentBook.Id);
            if (reviewedBook != null && reviewedBook.SelectedGenre == selectedGenre)
            {
                // Unselect the genre
                reviewedBook.SelectedGenre = null;

                // If the genre was selected and is now unselected, remove the book from the ReviewedBooks list
                ReviewedBooks.Remove(reviewedBook);

                StatusMessage = $"Genre '{selectedGenre}' unselected for '{CurrentBook.Title}'. Book removed from labeled list.";
            }
        }

        /// <summary>
        /// Skips the current book and accepts the genre with the highest probability.
        /// Adds the genre to ReviewedBooks and moves to the next book.
        /// </summary>
        private async Task SkipBook()
        {
            if (CurrentBook == null)
            {
                ErrorMessage = "No book selected.";
                return;
            }

            var selectedGenre = Predictions.OrderByDescending(g => g.Probability).FirstOrDefault()?.Name;

            if (selectedGenre != null)
            {
                var reviewedBook = ReviewedBooks.FirstOrDefault(b => b.BookId == CurrentBook.Id);
                if (reviewedBook == null)
                {
                    reviewedBook = new ReviewedBook
                    {
                        BookId = CurrentBook.Id,
                        Title = CurrentBook.Title,
                        SelectedGenre = selectedGenre
                    };
                    ReviewedBooks.Add(reviewedBook);
                }
                else
                {
                    reviewedBook.SelectedGenre = selectedGenre;
                }

                StatusMessage = $"Book '{CurrentBook.Title}' skipped. Genre '{selectedGenre}' accepted.";
                NextBook();
            }
        }

        /// <summary>
        /// Submits the reviewed books with selected genres to the API for update.
        /// </summary>
        private async Task SubmitReviewedBooks()
        {
            try
            {
                var updateResponse = await Http.PostAsJsonAsync($"{_configuration.GetSection("flaskApi").Value}/update_books_genres", ReviewedBooks);
                if (updateResponse.IsSuccessStatusCode)
                {
                    foreach (var reviewedBook in ReviewedBooks)
                    {
                        var bookToRemove = Books.FirstOrDefault(b => b.Id == reviewedBook.BookId);
                        if (bookToRemove != null)
                        {
                            Books.Remove(bookToRemove);
                        }
                    }

                    StatusMessage = "Genres updated successfully!";
                    ReviewedBooks.Clear();

                    CurrentBookIndex = 0;
                    SetCurrentBook();
                }
                else
                {
                    ErrorMessage = "Error updating genres.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }


        /// <summary>
        /// Advances to the next book in the list.
        /// </summary>
        private void NextBook()
        {
            if (CurrentBookIndex < Books.Count - 1)
            {
                CurrentBookIndex++;
                SetCurrentBook();
                StateHasChanged(); // Force re-render
            }
        }

        /// <summary>
        /// Moves to the previous book in the list.
        /// </summary>
        private void PreviousBook()
        {
            if (CurrentBookIndex > 0)
            {
                CurrentBookIndex--;
                SetCurrentBook();
                StateHasChanged(); // Force re-render
            }
        }

        /// <summary>
        /// Loads additional books from the API when needed.
        /// </summary>
        private async Task LoadMoreBooks()
        {
            await LoadBooksWithPredictions();
        }

        /// <summary>
        /// Checks if a genre has been selected for the current book.
        /// </summary>
        private bool IsGenreSelected(string genreName)
        {
            return ReviewedBooks.Any(b => b.BookId == CurrentBook.Id && b.SelectedGenre == genreName);
        }

    }
}
