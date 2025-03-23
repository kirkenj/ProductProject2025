using Application.Models.User;
using AuthService.Core.Application.Contracts.Infrastructure;
using AuthService.Core.Application.Contracts.Persistence;
using Cache.Contracts;
using CustomResponse;
using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand
{
    public class SendTokenToUpdateUserEmailComandHandler : IRequestHandler<SendTokenToUpdateUserEmailRequest, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;

        public SendTokenToUpdateUserEmailComandHandler(IUserRepository userRepository, ICustomMemoryCache memoryCache, IEmailSender emailSender, IPasswordGenerator passwordGenerator, IOptions<UpdateUserEmailSettings> options)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _passwordGenerator = passwordGenerator;
            _memoryCache = memoryCache;
            _updateUserEmailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Response<string>> Handle(SendTokenToUpdateUserEmailRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.UpdateUserEmailDto.Id);

            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(user.Id), true);
            }

            await _emailSender.SendEmailAsync(new Email
            {
                To = user.Email ?? throw new ApplicationException(),
                Body = $"Dear {user.Name}. Your email is being updated.",
                Subject = "Email is being updated",
            });

            string token = _passwordGenerator.Generate();

            bool isEmailSent = await _emailSender.SendEmailAsync(new Email
            {
                To = request.UpdateUserEmailDto.Email,
                Subject = "Change email confirmation",
                Body = string.Format(_updateUserEmailSettings.UpdateUserEmailMessageBodyFormat, token)
            });

            if (isEmailSent == false)
            {
                throw new ApplicationException("Email was not sent");
            }

            await _memoryCache.SetAsync(
                string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, token),
                request.UpdateUserEmailDto,
                TimeSpan.FromHours(_updateUserEmailSettings.EmailUpdateTimeOutHours));

            return Response<string>.OkResponse("Check emails to get further details", string.Empty);
        }
    }
}