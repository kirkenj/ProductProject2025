using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.RegisterConfirmCommand
{
    public class RegisterConfirmCommandHandler : IRequestHandler<RegisterConfirmCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly CreateUserSettings _createUserSettings;
        private readonly ILogger<RegisterConfirmCommandHandler> _logger;

        public RegisterConfirmCommandHandler(IUserRepository userRepository,
            ICustomMemoryCache memoryCache,
            IOptions<CreateUserSettings> createUserSettings,
            ILogger<RegisterConfirmCommandHandler> logger)

        {
            _userRepository = userRepository;
            _memoryCache = memoryCache;
            _createUserSettings = createUserSettings.Value;
            _logger = logger;
        }

        public async Task<Response<string>> Handle(RegisterConfirmCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string loginEmail = request.Email;

                string cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, loginEmail, request.Token);

                var cachedUserValue = await _memoryCache.GetAsync<Domain.Models.User>(cacheKey, cancellationToken);

                if (cachedUserValue == null)
                {
                    return Response<string>.NotFoundResponse("Invalid token or email");
                }

                await _userRepository.AddAsync(cachedUserValue, cancellationToken);
                await _memoryCache.RemoveAsync(cacheKey, cancellationToken);

                return Response<string>.OkResponse("Success", string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return Response<string>.ServerErrorResponse("Error :(");
            }
        }
    }
}