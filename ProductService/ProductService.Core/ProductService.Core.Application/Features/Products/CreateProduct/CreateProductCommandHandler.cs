using AutoMapper;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.ProductService;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Domain.Models;


namespace ProductService.Core.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Response<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAuthApiClientService _authClientService;
        private readonly IKafkaProducer<ProductCreated> _notificationProducer;

        public CreateProductCommandHandler(IProductRepository userRepository, IMapper mapper, IAuthApiClientService authClientService, IKafkaProducer<ProductCreated> notificationProducer)
        {
            _productRepository = userRepository;
            _mapper = mapper;
            _authClientService = authClientService;
            _notificationProducer = notificationProducer;
        }

        public async Task<Response<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);

            var producerId = request.ProducerId;

            var userResponse = await _authClientService.GetUser(producerId);
            if (userResponse.Result == null)
            {
                throw new ApplicationException($"Couldn't get user with id '{producerId}'");
            }

            await _productRepository.AddAsync(product);

            await _notificationProducer.ProduceAsync(new ProductCreated { ProductId = product.Id }, cancellationToken);
            
            return Response<Guid>.OkResponse(product.Id, $"Product created with id {product.Id}");
        }
    }
}
