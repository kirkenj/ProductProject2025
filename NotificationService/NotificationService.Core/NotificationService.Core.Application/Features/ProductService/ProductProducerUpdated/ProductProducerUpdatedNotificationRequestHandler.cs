using Clients.Adapters.AuthClient.Contracts;
using Clients.Adapters.ProductClient.Contracts;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    internal class ProductProducerUpdatedNotificationRequestHandler : NotificationRequestHandler<ProductProducerUpdatedNotificationRequest>
    {
        private readonly IAuthApiClientService _authApiClient;
        private readonly IProductClientService _productApiClient;

        public ProductProducerUpdatedNotificationRequestHandler(INotificationRepository repository, IAuthApiClientService authApiClient, IProductClientService productApiClient) : base(repository)
        {
            _authApiClient = authApiClient;
            _productApiClient = productApiClient;
        }

        protected override async Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(ProductProducerUpdatedNotificationRequest request)
        {
            var oldOwner = await _authApiClient.GetUser(request.OldProducerId);
            var newOwner = await _authApiClient.GetUser(request.NewProducerId);

            var product = await _productApiClient.GetProduct(request.ProductId);

            return
            [
                new ProductProducerUpdatedNotificationForOldOwner{
                    UserId = oldOwner.Result!.Id.ToString(),
                    ProductId = request.ProductId.ToString(),
                    UserDto = oldOwner.Result,
                    ProductDto = product.Result!,
                },
                new ProductProducerUpdatedNotificationForNewOwner{
                    UserId = newOwner.Result!.Id.ToString(),
                    ProductId = request.ProductId.ToString(),
                    UserDto = newOwner.Result!,
                    ProductDto = product.Result!,
                }
            ];
        }
    }
}
