using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;


namespace AuthService.Core.Application.Features.User.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComandHandler : IRequestHandler<ConfirmEmailChangeComand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;
        private readonly IKafkaProducer<UserEmailChanged> _emailChangedProducer;

        public ConfirmEmailChangeComandHandler(IUserRepository userRepository, ICustomMemoryCache memoryCache, IOptions<UpdateUserEmailSettings> options, IKafkaProducer<UserEmailChanged> emailChangedProducer)
        {
            _userRepository = userRepository;
            _memoryCache = memoryCache;
            _updateUserEmailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
            _emailChangedProducer = emailChangedProducer;
        }

        public async Task<Response<string>> Handle(ConfirmEmailChangeComand request, CancellationToken cancellationToken)
        {
            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            ChangeEmailRequest? cachedDetailsValue = await _memoryCache.GetAsync<ChangeEmailRequest>(cacheKey);

            if (cachedDetailsValue == null)
            {
                return Response<string>.NotFoundResponse("No email update request for your account found. Try to create a new request");
            }

            if (cachedDetailsValue.OtpToOldEmail != request.OtpToOldEmail
            || cachedDetailsValue.OtpToNewEmail != request.OtpToNewEmail)
            {
                return Response<string>.BadRequestResponse("Invalid token");
            }

            if (await _userRepository.GetAsync(new UserFilter() { AccurateEmail = cachedDetailsValue.NewEmail }) != null)
            {
                return Response<string>.BadRequestResponse("Email has already been taken");
            }

            Domain.Models.User? userToUpdate = await _userRepository.GetAsync(request.Id);

            if (userToUpdate == null)
            {
                return Response<string>.NotFoundResponse("User not found");
            }

            if (userToUpdate.Id != cachedDetailsValue.UserId)
            {
                throw new InvalidOperationException("Cached user id is not equal to id specified in request");
            }

            userToUpdate.Email = cachedDetailsValue.NewEmail;

            await _userRepository.UpdateAsync(userToUpdate);

            await _memoryCache.RemoveAsync(cacheKey);

            await _emailChangedProducer.ProduceAsync(new UserEmailChanged { UserId = userToUpdate.Id }, cancellationToken);

            return Response<string>.OkResponse("Email updated.", string.Empty);
        }
    }
}
