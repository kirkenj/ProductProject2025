using FluentValidation;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.DTOs.Product.Validators;

namespace ProductService.Core.Application.Features.Products.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(IAuthApiClientService authApiClientService, IProductRepository productRepository)
        {
            RuleFor(u => u.UpdateProductDto).NotNull().SetValidator(new UpdateProductDtoValidator(authApiClientService, productRepository));
        }
    }
}
