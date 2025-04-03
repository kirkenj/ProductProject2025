using AutoMapper;
using CustomResponse;
using MediatR;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Dtos;

namespace NotificationService.Core.Application.Features.NotificatioinService.GetNotifications
{
    public class GetNotificationsCommandHandler : IRequestHandler<GetNotificationsCommand, Response<IEnumerable<NotificationDto>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public GetNotificationsCommandHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;

        }

        public async Task<Response<IEnumerable<NotificationDto>>> Handle(GetNotificationsCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Filter);

            var selectResult = await _notificationRepository.GetPageContent(request.Filter, request.Page, request.PageSize);

            var result = _mapper.Map<IEnumerable<NotificationDto>>(selectResult);

            return Response<IEnumerable<NotificationDto>>.OkResponse(result, string.Empty);
        }
    }
}
