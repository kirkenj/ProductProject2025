using AutoMapper;
using CustomResponse;
using NSubstitute;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.DTOs.Product;
using ProductService.Core.Application.Features.Products.GetProducListDto;

namespace ProductService.Core.Application.Tests.Features.Product
{
    public class GetProductListHandlerTests
    {
        private readonly IProductRepository _producrRepository;
        private readonly IMapper _mapper;
        private readonly GetProductListHandler _handler;

        public GetProductListHandlerTests()
        {
            _producrRepository = Substitute.For<IProductRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetProductListHandler(_producrRepository, _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsOkAndDtoList()
        {
            // Arrange
            var request = new GetProductListDtoRequest
            {
                Page = 1,
                PageSize = 10,
                ProductFilter = new()
            };

            var productList = new List<Domain.Models.Product>();
            var productDtoList = new List<ProductListDto>();

            _producrRepository.GetPageContentAsync(Arg.Is(request.ProductFilter), Arg.Is(request.Page), Arg.Is(request.PageSize))
                .Returns(productList);

            _mapper.Map<List<ProductListDto>>(Arg.Is(productList)).Returns(productDtoList);

            var expectedResult = Response<List<ProductListDto>>.OkResponse(productDtoList, "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            Assert.Equal(expectedResult.Result, result.Result);
        }
    }
}