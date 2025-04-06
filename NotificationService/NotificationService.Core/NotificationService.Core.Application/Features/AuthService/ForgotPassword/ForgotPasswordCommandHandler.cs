using Clients.Adapters.AuthClient.Contracts;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NotificationService.Core.Application.Features.AuthService.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthApiClientService _authApiClient;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(IAuthApiClientService authApiClient, IEmailSender emailSender, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _authApiClient = authApiClient;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authApiClient.GetUser(request.UserId);
                if (user.Result == null)
                {
                    _logger.LogError("Couldn't get user with id {id}", request.UserId);
                    return;
                }

                var email = new Email()
                {
                    To = user.Result.Email,
                    Body = $"Your new password: \'{request.NewPassword}\'",
                    Subject = "Password recovery"
                };

                await _emailSender.SendEmailAsync(email, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
