using Clients.Adapters.AuthClient.Contracts;
using Clients.Adapters.ProductClient.Contracts;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated
{
    public class ProductDeletedNotificationRequestHandler : NotificationRequestHandler<ProductCreatedNotificationRequest>
    {
        private readonly IAuthApiClientService _authApiClient;
        private readonly IProductClientService _productApiClient;

        public ProductDeletedNotificationRequestHandler(INotificationRepository repository, IAuthApiClientService authApiClient, IProductClientService productApiClient) : base(repository)
        {
            _authApiClient = authApiClient;
            _productApiClient = productApiClient;
        }

        protected override async Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(ProductCreatedNotificationRequest request)
        {
            var productDtoTask = _productApiClient.GetProduct(request.ProductId);
            var userDtoTask = _authApiClient.GetUser(request.ProducerId);

            await Task.WhenAll(productDtoTask, userDtoTask);

            return [new ProductCreatedNotification
                {
                     ProductDto = productDtoTask.Result.Result,
                     UserDto = userDtoTask.Result.Result,
                     ProductId = request.ProductId.ToString(),
                     UserId = request.ProducerId.ToString(),
                }];
        }
    }
}
