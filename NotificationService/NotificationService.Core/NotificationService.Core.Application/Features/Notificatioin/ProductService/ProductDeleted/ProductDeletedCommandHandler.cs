using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted
{
    public class ProductDeletedCommandHandler : IRequestHandler<ProductDeletedCommand>
    {
        public Task Handle(ProductDeletedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
