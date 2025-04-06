using AutoMapper;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Logging;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Domain.Models;

namespace ProductService.Core.Application.Features.Products.Commands.UpdateProductCommand
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer<ProductProducerUpdated> _productProducerUpdatedProducer;
        private readonly IAuthApiClientService _authApiClientService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            IMapper mapper,
            IAuthApiClientService authApiClientService,
            IKafkaProducer<ProductProducerUpdated> productProducerUpdatedProducer,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productProducerUpdatedProducer = productProducerUpdatedProducer;
            _authApiClientService = authApiClientService;
            _logger = logger;
        }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Response<string>.NotFoundResponse(nameof(Product), true);
            }

            var oldProducerId = product.ProducerId;
            var newProducerId = request.ProducerId;

            if (oldProducerId != newProducerId)
            {
                var userRequest = await _authApiClientService.GetUser(request.ProducerId);
                if (userRequest.Result == null)
                {
                    var message = $"User not found ({request.ProducerId})";
                    _logger.LogWarning(message);
                    return Response<string>.BadRequestResponse(message);
                }
            }

            _mapper.Map(request, product);
            await _productRepository.UpdateAsync(product, cancellationToken);
            _logger.LogInformation("Product ({id}) updated", product.Id);

            if (oldProducerId != newProducerId)
            {
                _logger.LogInformation("Producing notification for users [{newProducerId}, {oldProducerId}]", newProducerId, oldProducerId);
                await _productProducerUpdatedProducer.ProduceAsync(new()
                {
                    ProductId = product.Id,
                    NewProducerId = newProducerId,
                    OldProducerId = oldProducerId
                }, cancellationToken);
            }

            return Response<string>.OkResponse("Success", "Product updated");
        }
    }
}