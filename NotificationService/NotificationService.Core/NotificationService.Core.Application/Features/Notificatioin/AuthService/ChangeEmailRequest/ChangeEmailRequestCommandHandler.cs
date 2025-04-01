using Clients.AuthApi;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ChangeEmailRequest
{
    public class ChangeEmailRequestCommandHandler : IRequestHandler<ChangeEmailRequestCommand>
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ChangeEmailRequestCommandHandler> _logger;

        public ChangeEmailRequestCommandHandler(IEmailSender emailSender, ILogger<ChangeEmailRequestCommandHandler> logger, IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(ChangeEmailRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authApiClient.UsersGETAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogError("Couldn't get user with id {id}", request.UserId);
                    return;
                }

                var emailToOldEmail = new Email()
                {
                    To = user.Email,
                    Body = $"Token to change email: \'{request.OtpToOldEmail}\'",
                    Subject = "Email change"
                };

                var emailToNewEmail = new Email()
                {
                    To = request.NewEmail,
                    Body = $"Token to change email: \'{request.OtpToNewEmail}\'",
                    Subject = "Email change"
                };

                await Task.WhenAll(
                    _emailSender.SendEmailAsync(emailToOldEmail, cancellationToken),
                    _emailSender.SendEmailAsync(emailToNewEmail, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
