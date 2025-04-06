using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.RegisterUserCommand;
using AuthService.Core.Application.Models.User.Settings;
using AutoMapper;
using Cache.Contracts;
using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User.Commands
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly IPasswordSetter _passwordSetter;
        private readonly CreateUserSettings _createUserSettings;
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IKafkaProducer<UserRegistrationRequestCreated> _userRegistrationRequestCreatedKafkaProducer;
        private readonly IMapper _mapper;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _createUserSettings = new CreateUserSettings()
            {
                ConfirmationTimeout = 12,
                DefaultRoleID = 2,
                KeyForRegistrationCachingFormat = "RegFormat {0} {1}"
            };

            _userRepository = Substitute.For<IUserRepository>();
            _passwordGenerator = Substitute.For<IPasswordGenerator>();
            _passwordSetter = Substitute.For<IPasswordSetter>();
            _memoryCache = Substitute.For<ICustomMemoryCache>();
            _userRegistrationRequestCreatedKafkaProducer = Substitute.For<IKafkaProducer<UserRegistrationRequestCreated>>();
            _mapper = Substitute.For<IMapper>();
            _handler = new RegisterUserCommandHandler(Options.Create(_createUserSettings), _userRepository, _mapper, _passwordGenerator, _memoryCache, _userRegistrationRequestCreatedKafkaProducer, _passwordSetter);
        }

        [Fact]
        public async Task Handle_EmailTaken_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                Address = "SomeAddress",
                ConfirmPassword = "Iwiwiwi",
                Password = "Iwiwiwi",
                Email = "SomeEmail",
                Name = "SomeName"
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == request.Email), Arg.Any<CancellationToken>())
                .Returns(new Domain.Models.User());

            var expectedResult = Response<string>.BadRequestResponse("Email is taken");
            
            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);

            await _memoryCache.DidNotReceive().SetAsync(
                Arg.Any<string>(),
                Arg.Any<Domain.Models.User>(),
                Arg.Is(TimeSpan.FromMinutes(_createUserSettings.ConfirmationTimeout)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ValidRequest_CachesRequestAndSendsNotification()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                Address = "SomeAddress",
                ConfirmPassword = "Iwiwiwi",
                Password = "Iwiwiwi",
                Email = "SomeEmail",
                Name = "SomeName"
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == request.Email), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var targetUser = new Domain.Models.User
            {
                Email = "Email"
            };

            _mapper.Map<Domain.Models.User>(Arg.Is(request)).Returns(targetUser);

            var token = "someToken";

            _passwordGenerator.Generate().Returns(token);

            var cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, targetUser.Email, token);

            var expectedResult = Response<string>.OkResponse($"Created user registration request. Further details sent on email", string.Empty);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);

            _passwordSetter.Received().SetPassword(Arg.Is(request.Password), Arg.Is(targetUser));

            await _memoryCache.Received().SetAsync(
                Arg.Is(cacheKey),
                Arg.Is(targetUser),
                Arg.Is(TimeSpan.FromMinutes(_createUserSettings.ConfirmationTimeout)),
                Arg.Any<CancellationToken>());

            await _userRegistrationRequestCreatedKafkaProducer.Received()
                .ProduceAsync(Arg.Is<UserRegistrationRequestCreated>(r => r.Email == targetUser.Email && r.Token == token),
                Arg.Any<CancellationToken>());
        }
    }
}