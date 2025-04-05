using AutoMapper;
using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.Features.Products.CreateProduct;
using ProductService.Core.Application.Models.UserClient;


namespace ProductService.Core.Application.Tests.Features.Product
{
    public class CreateProductCommandHandlerTests
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAuthApiClientService _authClientService;
        private readonly IKafkaProducer<ProductCreated> _notificationProducer;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _mapper = Substitute.For<IMapper>();
            _authClientService = Substitute.For<IAuthApiClientService>();
            _notificationProducer = Substitute.For<IKafkaProducer<ProductCreated>>();
            var logger = Substitute.For<ILogger<CreateProductCommandHandler>>();
            _handler = new CreateProductCommandHandler(_productRepository, _mapper, _authClientService, _notificationProducer, logger);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateProductCommand
            {
                ProducerId = Guid.NewGuid(),
            };

            var authServiceResponse = new ClientResponse<AuthClientUser?>()
            {
                Result = null
            };

            _authClientService.GetUser(Arg.Is(request.ProducerId)).Returns(authServiceResponse);

            var expectedResult = Response<Guid>.BadRequestResponse($"Couldn't get user with id '{request.ProducerId}'");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_UserFound_CreatesProductReturnsOk()
        {
            // Arrange
            var request = new CreateProductCommand
            {
                ProducerId = Guid.NewGuid(),
            };

            var authServiceResponse = new ClientResponse<AuthClientUser?>()
            {
                Result = new AuthClientUser()
            };

            var productToAdd = new Domain.Models.Product()
            {
                Id = Guid.NewGuid(),
            };

            _mapper.Map<Domain.Models.Product>(Arg.Is(request)).Returns(productToAdd);

            _authClientService.GetUser(Arg.Is(request.ProducerId)).Returns(authServiceResponse);

            var expectedResult = Response<Guid>.OkResponse(productToAdd.Id, $"Product created with id {productToAdd.Id}");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            await _productRepository.Received().AddAsync(Arg.Is(productToAdd), Arg.Any<CancellationToken>());
            await _notificationProducer.Received().ProduceAsync(Arg.Is<ProductCreated>(p =>
                p.ProductId == productToAdd.Id
                && p.ProducerId == productToAdd.ProducerId),
                Arg.Any<CancellationToken>());
        }
    }
}