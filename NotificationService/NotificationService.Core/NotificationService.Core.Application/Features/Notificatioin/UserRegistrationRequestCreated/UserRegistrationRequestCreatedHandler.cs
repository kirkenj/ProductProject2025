using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedHandler : IRequestHandler<UserRegistrationRequestCreatedCommand>
    {
        public Task Handle(UserRegistrationRequestCreatedCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
