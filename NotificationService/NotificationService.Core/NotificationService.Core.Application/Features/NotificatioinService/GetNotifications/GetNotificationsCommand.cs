using CustomResponse;
using MediatR;
using NotificationService.Core.Application.Models.Dtos;
using NotificationService.Core.Application.Models.Filters;

namespace NotificationService.Core.Application.Features.NotificatioinService.GetNotifications
{
    public class GetNotificationsCommand : IRequest<Response<IEnumerable<NotificationDto>>>
    {
        public NotificationFilter? Filter { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
