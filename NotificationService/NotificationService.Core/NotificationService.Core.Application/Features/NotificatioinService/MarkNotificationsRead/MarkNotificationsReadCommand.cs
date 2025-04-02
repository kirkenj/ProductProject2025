using CustomResponse;
using MediatR;
using NotificationService.Core.Application.Models.Filters;

namespace NotificationService.Core.Application.Features.NotificatioinService.MarkNotificationsRead
{
    public class MarkNotificationsReadCommand : IRequest<Response<string>>
    {
        public NotificationFilter? Filter { get; set; }
    }
}
