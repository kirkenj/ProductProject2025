﻿using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Domain.Models;

namespace ProductService.Core.Application.Features.Products.Commands.RemoveProductCommand
{
    public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, Response<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IKafkaProducer<ProductDeleted> _productDeletedProducer;

        public RemoveProductCommandHandler(IProductRepository productRepository, IKafkaProducer<ProductDeleted> productDeletedProducer)
        {
            _productRepository = productRepository;
            _productDeletedProducer = productDeletedProducer;
        }

        public async Task<Response<string>> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.Id, cancellationToken);
            if (product == null)
            {
                return Response<string>.NotFoundResponse(nameof(Product), true);
            }

            await _productRepository.DeleteAsync(product.Id, cancellationToken);

            await _productDeletedProducer.ProduceAsync(new()
            {
                Id = request.Id,
                Name = product.Name,
                OwnerId = product.ProducerId
            }, cancellationToken);

            return Response<string>.OkResponse("Ok", "Success");
        }
    }
}