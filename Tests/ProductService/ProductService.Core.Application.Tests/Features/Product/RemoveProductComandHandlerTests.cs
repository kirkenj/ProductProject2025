using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.Features.Products.Commands.RemoveProductCommand;

namespace ProductService.Core.Application.Tests.Features.Product
{
    public class RemoveProductComandHandlerTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IKafkaProducer<ProductDeleted> _productDeletedProducer;
        private readonly RemoveProductCommandHandler _handler;

        public RemoveProductComandHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _productDeletedProducer = Substitute.For<IKafkaProducer<ProductDeleted>>();
            _handler = new RemoveProductCommandHandler(_productRepository, _productDeletedProducer);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new RemoveProductCommand
            {
                Id = Guid.NewGuid(),
            };

            _productRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse(nameof(Domain.Models.Product), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_ProductFound_RemovesProductReturnsOk()
        {
            // Arrange
            var request = new RemoveProductCommand
            {
                Id = Guid.NewGuid(),
            };

            var targetProduct = new Domain.Models.Product()
            {
                Name = "SomeName",
                Id = Guid.NewGuid(),
            };

            _productRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targetProduct);

            var expectedResult = Response<string>.OkResponse("Ok", "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            await _productRepository.Received()
                .DeleteAsync(Arg.Is(targetProduct.Id), Arg.Any<CancellationToken>());

            await _productDeletedProducer.Received().ProduceAsync(Arg.Is<ProductDeleted>(p =>
                p.Id == request.Id
                && p.Name == targetProduct.Name
                && p.OwnerId == targetProduct.ProducerId),
              Arg.Any<CancellationToken>());

            Assert.Equivalent(expectedResult, result);
        }
    }
}