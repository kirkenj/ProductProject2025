using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated
{
    public class ProductCreatedCommandHandler : IRequestHandler<ProductCreatedCommand>
    {
        public Task Handle(ProductCreatedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
