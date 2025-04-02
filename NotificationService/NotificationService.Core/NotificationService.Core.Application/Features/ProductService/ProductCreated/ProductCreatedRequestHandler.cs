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
            var productDto = await _productApiClient.ProductGETAsync(request.ProductId);
            var userDto = await _authApiClient.UsersGETAsync(productDto.ProducerId);

            return [new ProductCreatedNotification
                {
                     ProductDto = productDto,
                     UserDto = userDto,
                     ProductId = productDto.Id.ToString(),
                     UserId = userDto.Id.ToString(),
                }];
        }
    }
}
