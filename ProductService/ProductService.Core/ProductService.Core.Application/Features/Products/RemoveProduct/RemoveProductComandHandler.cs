using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using ProductService.Core.Application.Contracts.Persistence;

namespace ProductService.Core.Application.Features.Products.RemoveProduct
{
    public class RemoveProductComandHandler : IRequestHandler<RemovePrductComand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IKafkaProducer<ProductDeleted> _productDeletedProducer;

        public RemoveProductComandHandler(IProductRepository productRepository, IKafkaProducer<ProductDeleted> productDeletedProducer)
        {
            _productRepository = productRepository;
            _productDeletedProducer = productDeletedProducer;
        }

        public async Task<Response<string>> Handle(RemovePrductComand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.ProductId);

            if (product == null)
            {
                return Response<string>.NotFoundResponse(nameof(product.Id), true);
            }

            await _productRepository.DeleteAsync(product.Id);

            await _productDeletedProducer.ProduceAsync(new()
            {
                Id = request.ProductId,
                Name = product.Name,
                OwnerId = product.ProducerId
            }, cancellationToken);

            return Response<string>.OkResponse("Ok", "Success");
        }
    }
}
