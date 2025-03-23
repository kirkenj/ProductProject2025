using AutoMapper;
using CustomResponse;
using EmailSender.Contracts;
using MediatR;
using ProductService.Core.Application.Contracts.Persistence;

namespace ProductService.Core.Application.Features.Products.RemoveProduct
{
    public class RemoveProductComandHandler : IRequestHandler<RemovePrductComand, Response<string>>
    {
        private readonly IProductRepository _productRepository;

        public RemoveProductComandHandler(IProductRepository productRepository, IEmailSender emailSender, IMapper mapper)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<string>> Handle(RemovePrductComand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(request.ProductId);

            if (product == null)
            {
                return Response<string>.NotFoundResponse(nameof(product.Id), true);
            }

            await _productRepository.DeleteAsync(product.Id);

            //UserDto ownerResult = _mapper.Map<UserDto>(await _authClientService.UsersGETAsync(product.ProducerId, cancellationToken));

            //if (ownerResult != null && ownerResult.Email != null)
            //{
            //    await _emailSender.SendEmailAsync(new Email
            //    {
            //        Subject = "Your product was removed",
            //        To = ownerResult.Email,
            //        Body = $"Your product with id '{product.Id}' was removed"
            //    });
            //}

            return Response<string>.OkResponse("Ok", "Success");
        }
    }
}
