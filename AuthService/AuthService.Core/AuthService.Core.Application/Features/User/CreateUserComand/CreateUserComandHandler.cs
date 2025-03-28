using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Infrastructure;
using AuthService.Core.Application.Models.User.Settings;
using AutoMapper;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.CreateUserComand
{
    public class CreateUserComandHandler : IRequestHandler<CreateUserCommand, Response<Guid>>
    {
        private readonly IPasswordSetter _passwordSetter;
        private readonly CreateUserSettings _createUserSettings;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IKafkaProducer<UserRegistrationRequestCreated> _userRegistrationRequestCreatedKafkaProducer;
        private readonly IMapper _mapper;

        public CreateUserComandHandler(IOptions<CreateUserSettings> createUserSettings,
            IMapper mapper,
            IPasswordGenerator passwordGenerator,
            ICustomMemoryCache memoryCache,
            IKafkaProducer<UserRegistrationRequestCreated> kafkaProducer,
            IPasswordSetter passwordSetter)
        {
            _createUserSettings = createUserSettings.Value;
            _passwordGenerator = passwordGenerator;
            _passwordSetter = passwordSetter;
            _memoryCache = memoryCache;
            _userRegistrationRequestCreatedKafkaProducer = kafkaProducer;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.User user = _mapper.Map<Domain.Models.User>(request);

            user.Id = Guid.NewGuid();
            user.Login = $"User{user.Id}";
            user.RoleID = _createUserSettings.DefaultRoleID;

            string password = _passwordGenerator.Generate();

            _passwordSetter.SetPassword(password, user);

            await _memoryCache.SetAsync(string.Format(_createUserSettings.KeyForRegistrationCachingFormat, user.Email), user, TimeSpan.FromHours(_createUserSettings.EmailConfirmationTimeoutHours));

            await _userRegistrationRequestCreatedKafkaProducer.ProduceAsync(new()
            {
                Email = user.Email,
                Password = password,
            }, cancellationToken);

            return Response<Guid>.OkResponse(user.Id, $"Created user registration request. Further details sent on email");
        }
    }
}