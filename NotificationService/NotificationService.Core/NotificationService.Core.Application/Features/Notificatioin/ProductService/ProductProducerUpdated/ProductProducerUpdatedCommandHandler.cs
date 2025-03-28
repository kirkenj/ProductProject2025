using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedCommandHandler : IRequestHandler<ProductProducerUpdatedCommand>
    {
        public Task Handle(ProductProducerUpdatedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
