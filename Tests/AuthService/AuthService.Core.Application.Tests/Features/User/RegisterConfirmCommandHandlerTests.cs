using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.RegisterConfirmCommand;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class RegisterConfirmCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly CreateUserSettings _createUserSettings;
        private readonly ILogger<RegisterConfirmCommandHandler> _logger;
        private readonly RegisterConfirmCommandHandler _handler;

        public RegisterConfirmCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _memoryCache = Substitute.For<ICustomMemoryCache>();
            _createUserSettings = new CreateUserSettings()
            {
                ConfirmationTimeout = 0,
                DefaultRoleID = 1,
                KeyForRegistrationCachingFormat = "SomeKey {0}{1}"
            };

            _logger = Substitute.For<ILogger<RegisterConfirmCommandHandler>>();
            _handler = new RegisterConfirmCommandHandler(_userRepository, _memoryCache, Options.Create(_createUserSettings), _logger);
        }

        [Fact]
        public async Task Handle_RequestNotFoundInCache_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterConfirmCommand()
            {
                Email = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };

            string cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, request.Email, request.Token);

            _memoryCache.GetAsync<Domain.Models.User>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse("Invalid token or email");
            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_RequestFoundInCache_AddsValueToRepositoryReturnsOk()
        {
            // Arrange
            var request = new RegisterConfirmCommand()
            {
                Email = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };

            string cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, request.Email, request.Token);

            var targetUser = new Domain.Models.User();

            _memoryCache.GetAsync<Domain.Models.User>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Returns(targetUser);


            var expectedResult = Response<string>.OkResponse("Success", string.Empty);
            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            await _userRepository.Received().AddAsync(Arg.Is(targetUser), Arg.Any<CancellationToken>());
            await _memoryCache.Received().RemoveAsync(Arg.Is(cacheKey), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var request = new RegisterConfirmCommand()
            {
                Email = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString()
            };

            string cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, request.Email, request.Token);

            _memoryCache.GetAsync<Domain.Models.User>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Throws<Exception>();

            var expectedResult = Response<string>.ServerErrorResponse("Error :(");
            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }
    }
}