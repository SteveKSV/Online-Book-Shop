namespace Client.Models
{
    public record class BookModel(
     string Id,
     string Title,
     string Description,
     string Genres,           
     decimal Price,
     string Publisher,
     string InfoLink,
     int RatingsCount,
     DateTime PublicationDate,
     string Authors,
     string PreviewLink, 
     string Image);

}
