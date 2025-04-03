using Clients.AuthApi;
using Clients.ProductService.Clients.ProductServiceClient;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated
{
    public class ProductDeletedNotificationRequestHandler : NotificationRequestHandler<ProductCreatedNotificationRequest>
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IProductApiClient _productApiClient;

        public ProductDeletedNotificationRequestHandler(INotificationRepository repository, IAuthApiClient authApiClient, IProductApiClient productApiClient) : base(repository)
        {
            _authApiClient = authApiClient;
            _productApiClient = productApiClient;
        }

        protected override async Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(ProductCreatedNotificationRequest request)
        {
            var productDtoTask = _productApiClient.ProductGETAsync(request.ProductId);
            var userDtoTask = _authApiClient.UsersGETAsync(request.ProducerId);

            await Task.WhenAll(productDtoTask, userDtoTask);

            return [new ProductCreatedNotification
                {
                     ProductDto = productDtoTask.Result,
                     UserDto = userDtoTask.Result,
                     ProductId = request.ProductId.ToString(),
                     UserId = request.ProducerId.ToString(),
                }];
        }
    }
}
