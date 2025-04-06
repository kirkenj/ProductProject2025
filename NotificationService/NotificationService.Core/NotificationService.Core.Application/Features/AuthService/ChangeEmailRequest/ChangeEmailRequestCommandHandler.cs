using Clients.Adapters.AuthClient.Contracts;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NotificationService.Core.Application.Features.AuthService.ChangeEmailRequest
{
    public class ChangeEmailRequestCommandHandler : IRequestHandler<ChangeEmailRequestCommand>
    {
        private readonly IAuthApiClientService _authApiClient;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ChangeEmailRequestCommandHandler> _logger;

        public ChangeEmailRequestCommandHandler(IEmailSender emailSender, ILogger<ChangeEmailRequestCommandHandler> logger, IAuthApiClientService authApiClient)
        {
            _authApiClient = authApiClient;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(ChangeEmailRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _authApiClient.GetUser(request.UserId);
                if (userResult.Result == null)
                {
                    _logger.LogError("Couldn't get user with id {id} ({code})", request.UserId, userResult.StatusCode);
                    return;
                }

                var emailToOldEmail = new Email()
                {
                    To = userResult.Result.Email,
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
