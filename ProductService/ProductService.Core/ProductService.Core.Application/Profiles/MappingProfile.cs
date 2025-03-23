using Application.DTOs.Product;
using AutoMapper;
using ProductService.Core.Domain.Models;

namespace ProductService.Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductListDto>();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            //CreateMap<UserDto, AuthClientUser>().ReverseMap();
            //CreateMap<RoleDto, AuthClientRole>().ReverseMap();
        }
    }
}
