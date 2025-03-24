using FluentValidation;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Features.Products.Contracts;

namespace ProductService.Core.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IAuthApiClientService authApiClient)
        {
            Include(new IEditProductValidator());
            RuleFor(x => x.ProducerId).MustAsync(async (id, token) =>
            {
                var result = await authApiClient.GetUser(id);
                return result.Success;
            });
        }
    }
}
