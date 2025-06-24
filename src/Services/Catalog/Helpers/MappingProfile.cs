using AutoMapper;
using Catalog.DTO;
using Catalog.Entities;

namespace Catalog.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Rating <-> RatingDTO
            CreateMap<Rating, RatingDTO>().ReverseMap();

            // Genre <-> GenreDTO
            CreateMap<Genre, GenreDTO>().ReverseMap();

            // Comment -> CommentDto (тільки в один бік)
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<Comment, AddCommentDTO>().ReverseMap();

            // Book -> BookDTO
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            // BookCreateDTO -> Book
            CreateMap<BookCreateDTO, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // генерується на бекенді
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(_ => 5))
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Genre, opt => opt.Ignore());

        }
    }
}
