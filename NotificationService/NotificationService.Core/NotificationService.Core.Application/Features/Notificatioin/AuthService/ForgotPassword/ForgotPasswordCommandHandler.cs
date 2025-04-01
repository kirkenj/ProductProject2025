using Clients.AuthApi;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly IAuthApiClient _authApiClient;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(IAuthApiClient authApiClient, IEmailSender emailSender, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _authApiClient = authApiClient;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authApiClient.UsersGETAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogError("Couldn't get user with id {id}", request.UserId);
                    return;
                }

                var email = new Email()
                {
                    To = user.Email,
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
