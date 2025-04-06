using AutoMapper;

namespace Clients.Adapters.ProductClient.Services
{
    public class ProductClientMappingProfile : Profile
    {
        public ProductClientMappingProfile()
        {
            CreateMap<Contracts.ProductDto, ProductApi.ProductDto>().ReverseMap();
        }
    }
}
