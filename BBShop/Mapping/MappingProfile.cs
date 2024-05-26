using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;

namespace BBShop.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserCreateDto, User>().ReverseMap();
        CreateMap<Store, StoreDto>().ReverseMap();
        CreateMap<StoreCreateDto, Store>().ReverseMap();
        // Other mappings
    }
}
