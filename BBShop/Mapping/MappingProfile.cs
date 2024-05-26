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
        CreateMap<UserUpdateDto, User>().ReverseMap();
        
        CreateMap<Store, StoreDto>().ReverseMap();
        CreateMap<StoreCreateDto, Store>().ReverseMap();
        CreateMap<StoreUpdateDto, Store>().ReverseMap();
        
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<ProductCreateDto, Product>().ReverseMap();
        CreateMap<ProductUpdateDto, Product>().ReverseMap();
        
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<OrderCreateDto, Order>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<OrderItemCreateDto, OrderItem>().ReverseMap();
        // Other mappings
    }
}
