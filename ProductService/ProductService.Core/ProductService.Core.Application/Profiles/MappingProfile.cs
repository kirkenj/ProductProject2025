using AutoMapper;
using ProductService.Core.Application.DTOs.Product;
using ProductService.Core.Application.Features.Products.CreateProduct;
using ProductService.Core.Application.Features.Products.UpdateProduct;
using ProductService.Core.Domain.Models;

namespace ProductService.Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductListDto>();
            CreateMap<Product, CreateProductCommand>().ReverseMap();
            CreateMap<Product, UpdateProductCommand>().ReverseMap();
        }
    }
}
