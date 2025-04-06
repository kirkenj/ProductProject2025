using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.Commands.SendTokenToUpdateUserEmailRequest
{
    public class SendTokenToUpdateUserEmailCommandHandler : IRequestHandler<SendTokenToUpdateUserEmailCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducer<ChangeEmailRequest> _changeEmailRequestCreatedProducer;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;

        public SendTokenToUpdateUserEmailCommandHandler(IUserRepository userRepository, ICustomMemoryCache memoryCache, IPasswordGenerator passwordGenerator, IOptions<UpdateUserEmailSettings> options, IKafkaProducer<ChangeEmailRequest> changeEmailRequestCreatedProducer)
        {
            _userRepository = userRepository;
            _changeEmailRequestCreatedProducer = changeEmailRequestCreatedProducer;
            _passwordGenerator = passwordGenerator;
            _memoryCache = memoryCache;
            _updateUserEmailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Response<string>> Handle(SendTokenToUpdateUserEmailCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetAsync(request.Id, cancellationToken) == null)
            {
                return Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);
            }

            if (await _userRepository.GetAsync(new UserFilter { AccurateEmail = request.Email }, cancellationToken) != null)
            {
                return Response<string>.BadRequestResponse("Email is taken");
            }

            var changeEmailRequest = new ChangeEmailRequest
            {
                OtpToNewEmail = _passwordGenerator.Generate(),
                UserId = request.Id,
                NewEmail = request.Email,
                OtpToOldEmail = _passwordGenerator.Generate()
            };

            await _memoryCache.SetAsync(
                string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id),
                changeEmailRequest,
                TimeSpan.FromHours(_updateUserEmailSettings.EmailUpdateTimeOutHours),
                cancellationToken);

            await _changeEmailRequestCreatedProducer.ProduceAsync(changeEmailRequest, cancellationToken);

            return Response<string>.OkResponse("Check emails to get further details", string.Empty);
        }
    }
}