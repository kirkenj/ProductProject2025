using AutoMapper;
using CustomResponse;
using MediatR;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.DTOs.Product;

namespace ProductService.Core.Application.Features.Products.GetProductDetail
{
    public class GetProductDtoHandler : IRequestHandler<GetProductDtoRequest, Response<ProductDto>>
    {
        private readonly IProductRepository _producrRepository;
        private readonly IMapper _mapper;

        public GetProductDtoHandler(IProductRepository userRepository, IMapper mapper)
        {
            _producrRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<ProductDto>> Handle(GetProductDtoRequest request, CancellationToken cancellationToken)
        {
            var product = await _producrRepository.GetAsync(request.Id, cancellationToken);

            return product == null ?
                Response<ProductDto>.NotFoundResponse(nameof(request.Id), true)
                : Response<ProductDto>.OkResponse(_mapper.Map<ProductDto>(product), "Success");
        }
    }
}