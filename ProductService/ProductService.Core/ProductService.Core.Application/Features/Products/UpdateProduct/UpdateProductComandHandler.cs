using AutoMapper;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using ProductService.Core.Application.Contracts.Persistence;

namespace ProductService.Core.Application.Features.Products.UpdateProduct
{
    public class UpdateProductComandHandler : IRequestHandler<UpdateProductCommand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IKafkaProducer<ProductProducerUpdated> _productProducerUpdatedProducer;

        public UpdateProductComandHandler(IProductRepository productRepository, IMapper mapper, IKafkaProducer<ProductProducerUpdated> productProducerUpdatedProducer)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productProducerUpdatedProducer = productProducerUpdatedProducer;
        }

        public async Task<Response<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.Id);
            if (product == null)
            {
                return Response<string>.NotFoundResponse(nameof(product.Id), true);
            }

            _mapper.Map(request, product);
            await _productRepository.UpdateAsync(product);

            var newOwnerId = request.ProducerId;
            if (product.ProducerId != newOwnerId)
            {
                await _productProducerUpdatedProducer.ProduceAsync(new() { ProductId = product.Id, NewProducerId = newOwnerId, OldProducerId = product.ProducerId }, cancellationToken);
            }

            return Response<string>.OkResponse("Success", "Product updated");
        }
    }
}
