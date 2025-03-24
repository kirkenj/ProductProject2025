using AutoMapper;
using CustomResponse;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
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
        private readonly IEmailSender _emailSender;

        public CreateProductCommandHandler(IProductRepository userRepository, IMapper mapper, IAuthApiClientService authClientService, IEmailSender emailSender)
        {
            _productRepository = userRepository;
            _mapper = mapper;
            _authClientService = authClientService;
            _emailSender = emailSender;
        }

        public async Task<Response<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);

            var addTask = _productRepository.AddAsync(product);

            var msgTask = Task.Run(async () =>
            {
                var producerId = request.ProducerId;

                var userResponse = await _authClientService.GetUser(producerId);

                var user = userResponse.Result ?? throw new ApplicationException($"Couldn't get user with id '{producerId}'");

                if (user.Email != null)
                {
                    await _emailSender.SendEmailAsync(new Email
                    {
                        Body = $"You added a product with id '{product.Id}'",
                        Subject = "Product creation",
                        To = user.Email
                    });
                }
            }, cancellationToken);

            await Task.WhenAll(msgTask, msgTask);

            return Response<Guid>.OkResponse(product.Id, $"Product created with id {product.Id}");
        }
    }
}
