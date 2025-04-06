using AutoMapper;
using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.Features.Products.Commands.UpdateProductCommand;
using ProductService.Core.Application.Models.UserClient;

namespace ProductService.Core.Application.Tests.Features.Product
{
    public class UpdateProductComandHandlerTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer<ProductProducerUpdated> _productProducerUpdatedProducer;
        private readonly IAuthApiClientService _authApiClientService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductComandHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _mapper = Substitute.For<IMapper>();
            _productProducerUpdatedProducer = Substitute.For<IKafkaProducer<ProductProducerUpdated>>();
            _authApiClientService = Substitute.For<IAuthApiClientService>();
            _logger = Substitute.For<ILogger<UpdateProductCommandHandler>>();
            _handler = new UpdateProductCommandHandler(_productRepository, _mapper, _authApiClientService, _productProducerUpdatedProducer, _logger);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateProductCommand
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
        public async Task Handle_OwnerUpdatedNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                ProducerId = Guid.NewGuid(),
            };

            var targerProduct = new Domain.Models.Product
            {
                ProducerId = Guid.NewGuid(),
            };

            _productRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targerProduct);

            _authApiClientService.GetUser(Arg.Is(request.ProducerId))
                .Returns(new ClientResponse<AuthClientUser?> { Result = null });

            var expectedResult = Response<string>.BadRequestResponse($"User not found ({request.ProducerId})");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_OwnerUpdatedFound_ProducesNotificationReturnsOk()
        {
            // Arrange
            var request = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                ProducerId = Guid.NewGuid(),
            };

            var targerProduct = new Domain.Models.Product
            {
                ProducerId = Guid.NewGuid(),
            };

            _productRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targerProduct);

            _authApiClientService.GetUser(Arg.Is(request.ProducerId))
                .Returns(new ClientResponse<AuthClientUser?> { Result = new() });

            var expectedResult = Response<string>.OkResponse("Success", "Product updated");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            _mapper.Received().Map(Arg.Is(request), Arg.Is(targerProduct));
            await _productRepository.Received().UpdateAsync(Arg.Is(targerProduct), Arg.Any<CancellationToken>());
            await _productProducerUpdatedProducer.Received().ProduceAsync(Arg.Is<ProductProducerUpdated>(p =>

                p.ProductId == targerProduct.Id
                && p.NewProducerId == request.ProducerId
                && p.OldProducerId == targerProduct.ProducerId
            ), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_OwnerNotUpdated_ReturnsOk()
        {
            // Arrange
            var request = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                ProducerId = Guid.NewGuid(),
            };

            var targerProduct = new Domain.Models.Product
            {
                ProducerId = request.ProducerId,
            };

            _productRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targerProduct);

            var expectedResult = Response<string>.OkResponse("Success", "Product updated");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            _mapper.Received().Map(Arg.Is(request), Arg.Is(targerProduct));
            await _productRepository.Received().UpdateAsync(Arg.Is(targerProduct), Arg.Any<CancellationToken>());
            await _productProducerUpdatedProducer.DidNotReceive().ProduceAsync(Arg.Any<ProductProducerUpdated>(), Arg.Any<CancellationToken>());
        }
    }
}