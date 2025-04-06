using AutoMapper;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Logging;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Domain.Models;


namespace ProductService.Core.Application.Features.Products.Commands.CreateProductCommand
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAuthApiClientService _authClientService;
        private readonly IKafkaProducer<ProductCreated> _notificationProducer;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            IAuthApiClientService authClientService,
            IKafkaProducer<ProductCreated> notificationProducer,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _authClientService = authClientService;
            _notificationProducer = notificationProducer;
            _logger = logger;
        }

        public async Task<Response<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var producerId = request.ProducerId;

            var userResponse = await _authClientService.GetUser(producerId);
            if (userResponse.Result == null)
            {
                var message = $"Couldn't get user with id '{producerId}'";
                _logger.LogWarning(message);
                return Response<Guid>.BadRequestResponse(message);
            }

            Product product = _mapper.Map<Product>(request);

            await _productRepository.AddAsync(product, cancellationToken);

            await _notificationProducer.ProduceAsync(new ProductCreated { ProductId = product.Id, ProducerId = product.ProducerId }, cancellationToken);

            return Response<Guid>.OkResponse(product.Id, $"Product created with id {product.Id}");
        }
    }
}