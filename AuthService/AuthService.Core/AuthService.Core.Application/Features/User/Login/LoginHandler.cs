using System.Text;
using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.DTOs;
using AuthService.Core.Application.Models.User.Settings;
using AutoMapper;
using Cache.Contracts;
using CustomResponse;
using HashProvider.Contracts;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.Login
{
    public class LoginHandler : IRequestHandler<LoginRequest, Response<UserDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHashProvider _hashProvider;
        private readonly IMapper _mapper;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly IKafkaProducer<AccountConfirmed> _accountConfirmedProducer;
        private readonly CreateUserSettings _createUserSettings;

        public LoginHandler(IUserRepository userRepository,
            IRoleRepository roleRepository,
            IHashProvider hashProvider,
            IMapper mapper,
            ICustomMemoryCache memoryCache,
            IKafkaProducer<AccountConfirmed> accountConfirmedProducer,
            IOptions<CreateUserSettings> createUserSettings)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _hashProvider = hashProvider;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _accountConfirmedProducer = accountConfirmedProducer;
            _createUserSettings = createUserSettings.Value;
        }

        public async Task<Response<UserDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            string loginEmail = request.Email;

            string cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, loginEmail);

            Domain.Models.User? cachedUserValue = await _memoryCache.GetAsync<Domain.Models.User>(cacheKey);

            bool isRegistration = cachedUserValue != null;

            var userToHandle = isRegistration ?
                cachedUserValue
                : await _userRepository.GetAsync(new UserFilter { AccurateEmail = loginEmail });

            if (userToHandle == null) return Response<UserDto>.BadRequestResponse("Wrong password or email");

            _hashProvider.HashAlgorithmName = userToHandle.HashAlgorithm;
            _hashProvider.Encoding = Encoding.GetEncoding(userToHandle.StringEncoding);

            string loginPasswordHash = _hashProvider.GetHash(request.Password);

            if (loginPasswordHash != userToHandle.PasswordHash)
            {
                return Response<UserDto>.BadRequestResponse("Wrong password or email");
            }

            if (isRegistration == true)
            {
                await RegisterUser(userToHandle, cancellationToken);
                await _memoryCache.RemoveAsync(cacheKey);
                userToHandle.Role = await _roleRepository.GetAsync(userToHandle.RoleID) ??
                    throw new ApplicationException($"Couldn't get role with id \'{userToHandle.RoleID}\'");
            }

            UserDto userDto = _mapper.Map<UserDto>(userToHandle);

            return Response<UserDto>.OkResponse(userDto, "Success");
        }

        private async Task RegisterUser(Domain.Models.User userToHandle, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(userToHandle);

            await _accountConfirmedProducer.ProduceAsync(new() { UserId = userToHandle.Id }, cancellationToken);
        }
    }
}
