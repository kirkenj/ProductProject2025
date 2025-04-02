using Clients.AuthApi;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted
{
    public class ProductDeletedNotificationRequestHandler : NotificationRequestHandler<ProductDeletedNotificationRequest>
    {
        private readonly IAuthApiClient _authApiClient;

        public ProductDeletedNotificationRequestHandler(INotificationRepository repository, IAuthApiClient authApiClient) : base(repository)
        {
            _authApiClient = authApiClient;
        }

        protected override async Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(ProductDeletedNotificationRequest request)
        {
            var userDto = await _authApiClient.UsersGETAsync(request.Id);

            return [new ProductDeletedNotification
            {
                ProductId = request.Id.ToString(),
                UserId = request.OwnerId.ToString(),
                UserDto = userDto
            }];
        }
    }
}
