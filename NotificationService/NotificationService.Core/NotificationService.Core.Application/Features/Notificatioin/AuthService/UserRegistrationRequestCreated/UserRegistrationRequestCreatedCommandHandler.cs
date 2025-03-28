using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommandHandler : IRequestHandler<UserRegistrationRequestCreatedCommand>
    {
        public Task Handle(UserRegistrationRequestCreatedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
