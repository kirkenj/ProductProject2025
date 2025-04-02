using Clients.AuthApi;
using Clients.ProductService.Clients.ProductServiceClient;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    internal class ProductProducerUpdatedNotificationRequestHandler : NotificationRequestHandler<ProductProducerUpdatedNotificationRequest>
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IProductApiClient _productApiClient;

        public ProductProducerUpdatedNotificationRequestHandler(INotificationRepository repository, IAuthApiClient authApiClient, IProductApiClient productApiClient) : base(repository)
        {
            _authApiClient = authApiClient;
            _productApiClient = productApiClient;
        }

        protected override async Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(ProductProducerUpdatedNotificationRequest request)
        {
            var oldOwner = await _authApiClient.UsersGETAsync(request.OldProducerId);
            var newOwner = await _authApiClient.UsersGETAsync(request.NewProducerId);

            var product = await _productApiClient.ProductGETAsync(request.ProductId);

            return
            [
                new ProductProducerUpdatedNotificationForOldOwner{
                    UserId = oldOwner.Id.ToString(),
                    ProductId = request.ProductId.ToString(),
                    UserDto = oldOwner,
                    ProductDto = product,
                },
                new ProductProducerUpdatedNotificationForNewOwner{
                    UserId = newOwner.Id.ToString(),
                    ProductId = request.ProductId.ToString(),
                    UserDto = newOwner,
                    ProductDto = product,
                }
            ];
        }
    }
}
