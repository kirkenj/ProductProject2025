using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;

namespace NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommandHandler : IRequestHandler<UserRegistrationRequestCreatedCommand>
    {
        private readonly IEmailSender _emailSender;

        public UserRegistrationRequestCreatedCommandHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Handle(UserRegistrationRequestCreatedCommand request, CancellationToken cancellationToken)
        {
            var email = new Email
            {
                To = request.Email,
                Subject = "Registration",
                Body = $"Confirm your email by logging in with credentials: \r\nEmail: {request.Email}\r\nPassword: {request.Password}"
            };

            await _emailSender.SendEmailAsync(email, cancellationToken);
        }
    }
}
