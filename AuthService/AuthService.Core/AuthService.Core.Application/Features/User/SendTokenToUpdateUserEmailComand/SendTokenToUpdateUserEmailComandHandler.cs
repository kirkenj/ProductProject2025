using AuthService.Core.Application.Contracts.Infrastructure;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand
{
    public class SendTokenToUpdateUserEmailComandHandler : IRequestHandler<SendTokenToUpdateUserEmailRequest, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducer<ChangeEmailRequest> _changeEmailRequestCreatedProducer;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;

        public SendTokenToUpdateUserEmailComandHandler(IUserRepository userRepository, ICustomMemoryCache memoryCache, IPasswordGenerator passwordGenerator, IOptions<UpdateUserEmailSettings> options, IKafkaProducer<ChangeEmailRequest> changeEmailRequestCreatedProducer)
        {
            _userRepository = userRepository;
            _changeEmailRequestCreatedProducer = changeEmailRequestCreatedProducer;
            _passwordGenerator = passwordGenerator;
            _memoryCache = memoryCache;
            _updateUserEmailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Response<string>> Handle(SendTokenToUpdateUserEmailRequest request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.Id);

            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(user.Id), true);
            }

            var changeEmailRequest = new ChangeEmailRequest
            {
                OtpToNewEmail = _passwordGenerator.Generate(),
                UserId = user.Id,
                NewEmail = request.Email,
                OtpToOldEmail = _passwordGenerator.Generate()
            };

            await _memoryCache.SetAsync(
                string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, user.Id),
                changeEmailRequest,
                TimeSpan.FromHours(_updateUserEmailSettings.EmailUpdateTimeOutHours));
            
            await _changeEmailRequestCreatedProducer.ProduceAsync(changeEmailRequest, cancellationToken);

            return Response<string>.OkResponse("Check emails to get further details", string.Empty);
        }
    }
}