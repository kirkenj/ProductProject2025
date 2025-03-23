using Application.DTOs.Product;
using AutoMapper;
using CustomResponse;
using MediatR;
using ProductService.Core.Application.Contracts.Persistence;

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
            var user = await _producrRepository.GetAsync(request.Id);

            if (user == null)
            {
                return Response<ProductDto>.NotFoundResponse(nameof(request.Id), true);
            }

            return Response<ProductDto>.OkResponse(_mapper.Map<ProductDto>(user), "Success");
        }
    }
}
