using AutoMapper;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.DTOs.Product;
using ProductService.Core.Application.Features.Products.GetProductDetail;

namespace ProductService.Core.Application.Tests.Features.Product
{
    public class GetProductDtoHandlerTests
    {
        private readonly IProductRepository _producrRepository;
        private readonly IMapper _mapper;
        private readonly GetProductDtoHandler _handler;

        public GetProductDtoHandlerTests()
        {
            _producrRepository = Substitute.For<IProductRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetProductDtoHandler(_producrRepository, _mapper);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new GetProductDtoRequest
            {
                Id = Guid.NewGuid(),
            };

            _producrRepository.GetAsync(Arg.Is(request.Id)).ReturnsNull();

            var expectedResult = Response<ProductDto>.NotFoundResponse(nameof(request.Id), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_UserFound_ReturnsMappedValue()
        {
            // Arrange
            var request = new GetProductDtoRequest
            {
                Id = Guid.NewGuid(),
            };

            var targerProduct = new Domain.Models.Product();

            _producrRepository.GetAsync(Arg.Is(request.Id)).Returns(targerProduct);

            var targetProductDto = new ProductDto();

            _mapper.Map<ProductDto>(Arg.Is(targerProduct)).Returns(targetProductDto);

            var expectedResult = Response<ProductDto>.OkResponse(targetProductDto, "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }
    }
}