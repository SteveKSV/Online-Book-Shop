using AutoMapper;
using Identity.Entities;
using Identity.Models;

namespace Identity.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
              .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserType.Name));
            CreateMap<User, LoginDTO>();
            CreateMap<User, RegisterDTO>();
        }
    }
}
