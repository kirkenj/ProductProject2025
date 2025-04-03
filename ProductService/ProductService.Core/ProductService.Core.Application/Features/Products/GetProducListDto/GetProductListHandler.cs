using AutoMapper;
using CustomResponse;
using MediatR;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.DTOs.Product;

namespace ProductService.Core.Application.Features.Products.GetProducListDto
{
    public class GetProductListHandler : IRequestHandler<GetProductListDtoRequest, Response<IEnumerable<ProductListDto>>>
    {
        private readonly IProductRepository _producrRepository;
        private readonly IMapper _mapper;

        public GetProductListHandler(IProductRepository productRepository, IMapper mapper)
        {
            _producrRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<ProductListDto>>> Handle(GetProductListDtoRequest request, CancellationToken cancellationToken)
        {
            var result = await _producrRepository.GetPageContentAsync(request.ProductFilter, request.Page, request.PageSize);

            return Response<IEnumerable<ProductListDto>>.OkResponse(_mapper.Map<List<ProductListDto>>(result), "Success");
        }
    }
}
