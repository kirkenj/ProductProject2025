using System.Text.Json;
using CustomResponse;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Contracts.Persistence;

namespace NotificationService.Core.Application.Features.NotificatioinService.MarkNotificationsRead
{
    public class MarkNotificationsReadCommandHandler : IRequestHandler<MarkNotificationsReadCommand, Response<string>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<MarkNotificationsReadCommandHandler> _logger;

        public MarkNotificationsReadCommandHandler(INotificationRepository notificationRepository, ILogger<MarkNotificationsReadCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }


        public async Task<Response<string>> Handle(MarkNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if ((request?.Filter?.Ids?.Count() ?? 0) == 0)
                {
                    return Response<string>.OkResponse("Specified filter is empty", string.Empty);
                }

                _logger.LogInformation("Sending request to repository with filter: {0}", JsonSerializer.Serialize(request!.Filter));
                var notificationsToMark = await _notificationRepository.GetPageContent(request!.Filter!, cancellationToken: cancellationToken);

                _logger.LogInformation("Got notifications with ids: {0}; Starting update process", JsonSerializer.Serialize(notificationsToMark.Select(n => n.Id)));
                var updateTasks = notificationsToMark.Select(notification =>
                {
                    notification.IsRead = true;
                    return _notificationRepository.UpdateAsync(notification);
                });

                await Task.WhenAll(updateTasks);

                _logger.LogInformation("Success");
                return Response<string>.OkResponse("Notifications updated", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return Response<string>.ServerErrorResponse("Server error");
            }
        }
    }
}
