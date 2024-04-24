namespace Client.Models
{
    public record class BookModel(string Id,
        string Title,
        string Description,
        string Genre,
        decimal Price,
        int NumberOfPages,
        string LanguageName,
        string PublisherName,
        DateTime PublicationDate,
        string AuthorName,
        string Image);
}
