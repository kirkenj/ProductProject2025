using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;


namespace AuthService.Core.Application.Features.User.Commands.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComandHandler : IRequestHandler<ConfirmEmailChangeComand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;

        public ConfirmEmailChangeComandHandler(IUserRepository userRepository, ICustomMemoryCache memoryCache, IOptions<UpdateUserEmailSettings> options)
        {
            _userRepository = userRepository;
            _memoryCache = memoryCache;
            _updateUserEmailSettings = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Response<string>> Handle(ConfirmEmailChangeComand request, CancellationToken cancellationToken)
        {
            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);

            ChangeEmailRequest? cachedDetailsValue = await _memoryCache.GetAsync<ChangeEmailRequest>(cacheKey, cancellationToken);
            if (cachedDetailsValue == null)
            {
                return Response<string>.NotFoundResponse("No email update request for your account found. Try to create a new request");
            }

            if (request.Id != cachedDetailsValue.UserId ||
               cachedDetailsValue.OtpToOldEmail != request.OtpToOldEmail
            || cachedDetailsValue.OtpToNewEmail != request.OtpToNewEmail)
            {
                return Response<string>.BadRequestResponse("Invalid token");
            }

            if (await _userRepository.GetAsync(new UserFilter() { AccurateEmail = cachedDetailsValue.NewEmail }, cancellationToken) != null)
            {
                return Response<string>.BadRequestResponse("Email has already been taken");
            }

            Domain.Models.User? userToUpdate = await _userRepository.GetAsync(request.Id, cancellationToken);
            if (userToUpdate == null)
            {
                return Response<string>.NotFoundResponse("User not found");
            }

            userToUpdate.Email = cachedDetailsValue.NewEmail;

            await _userRepository.UpdateAsync(userToUpdate, cancellationToken);

            await _memoryCache.RemoveAsync(cacheKey, cancellationToken);

            return Response<string>.OkResponse("Email updated.", string.Empty);
        }
    }
}
