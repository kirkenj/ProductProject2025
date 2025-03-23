using FluentValidation;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.DTOs.Product.Validators;

namespace ProductService.Core.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IAuthApiClientService authApiClient)
        {
            RuleFor(o => o.CreateProductDto).NotNull().SetValidator(new CreateProductDtoValidator(authApiClient));
        }
    }
}
