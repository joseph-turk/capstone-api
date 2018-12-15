using AutoMapper;
using CapstoneApi.Models;
using CapstoneApi.Dtos;

namespace CapstoneApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}