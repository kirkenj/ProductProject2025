using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.ConfirmEmailChangeComand;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class ConfirmEmailChangeComandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;
        private readonly ConfirmEmailChangeComandHandler _handler;

        public ConfirmEmailChangeComandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _memoryCache = Substitute.For<ICustomMemoryCache>();
            _updateUserEmailSettings = new() { EmailUpdateTimeOutHours = 1, UpdateUserEmailCacheKeyFormat = "{0}qwertyuiop" };
            _handler = new(_userRepository, _memoryCache, Options.Create(_updateUserEmailSettings));
        }

        [Fact]
        public async Task HandleAsync_NoDetailsInCache_ReturnsNotFound()
        {
            // Arrange
            var request = new ConfirmEmailChangeComand()
            {
                Id = Guid.NewGuid(),
                OtpToNewEmail = "SomeOtp1",
                OtpToOldEmail = "SomeOtp1"
            };

            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            _memoryCache.GetAsync<ChangeEmailRequest>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse("No email update request for your account found. Try to create a new request");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        public async Task HandleAsync_InvalidTokens_ReturnsInvalidToken(bool userIdValid, bool otpNewValid, bool otpOldValid)
        {
            // Arrange
            var request = new ConfirmEmailChangeComand()
            {
                Id = Guid.NewGuid(),
                OtpToNewEmail = "SomeOtp1",
                OtpToOldEmail = "SomeOtp2"
            };

            var cachedData = new ChangeEmailRequest
            {
                UserId = userIdValid ? request.Id : Guid.NewGuid(),
                OtpToNewEmail = otpNewValid ? request.OtpToNewEmail : Guid.NewGuid().ToString(),
                OtpToOldEmail = otpOldValid ? request.OtpToOldEmail : Guid.NewGuid().ToString(),
            };


            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            _memoryCache.GetAsync<ChangeEmailRequest>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Returns(cachedData);

            var expectedResult = Response<string>.BadRequestResponse("Invalid token");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task HandleAsync_EmailTaken_ReturnsEmailTaken()
        {
            // Arrange
            var request = new ConfirmEmailChangeComand()
            {
                Id = Guid.NewGuid(),
                OtpToNewEmail = "SomeOtp1",
                OtpToOldEmail = "SomeOtp2"
            };

            var cachedData = new ChangeEmailRequest
            {
                UserId = request.Id,
                NewEmail = "SomeNewEmail",
                OtpToNewEmail = request.OtpToNewEmail,
                OtpToOldEmail = request.OtpToOldEmail
            };


            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            _memoryCache.GetAsync<ChangeEmailRequest>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Returns(cachedData);

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == cachedData.NewEmail))
                .Returns(new Domain.Models.User());

            var expectedResult = Response<string>.BadRequestResponse("Email has already been taken");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task HandleAsync_TargetUserNotFound_ReturnsEmailTaken()
        {
            // Arrange
            var request = new ConfirmEmailChangeComand()
            {
                Id = Guid.NewGuid(),
                OtpToNewEmail = "SomeOtp1",
                OtpToOldEmail = "SomeOtp2"
            };

            var cachedData = new ChangeEmailRequest
            {
                UserId = request.Id,
                NewEmail = "SomeNewEmail",
                OtpToNewEmail = request.OtpToNewEmail,
                OtpToOldEmail = request.OtpToOldEmail
            };


            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            _memoryCache.GetAsync<ChangeEmailRequest>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Returns(cachedData);

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == cachedData.NewEmail))
            .ReturnsNull();

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>()).ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse("User not found");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task HandleAsync_TargetUserFound_UpdatesUserRemovesKeyFromCache()
        {
            // Arrange
            var request = new ConfirmEmailChangeComand()
            {
                Id = Guid.NewGuid(),
                OtpToNewEmail = "SomeOtp1",
                OtpToOldEmail = "SomeOtp2"
            };

            var cachedData = new ChangeEmailRequest
            {
                UserId = request.Id,
                NewEmail = "SomeNewEmail",
                OtpToNewEmail = request.OtpToNewEmail,
                OtpToOldEmail = request.OtpToOldEmail
            };


            var cacheKey = string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id);
            _memoryCache.GetAsync<ChangeEmailRequest>(Arg.Is(cacheKey), Arg.Any<CancellationToken>())
                .Returns(cachedData);

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == cachedData.NewEmail))
            .ReturnsNull();

            var targetUser = new Domain.Models.User { };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targetUser);

            var expectedResult = Response<string>.OkResponse("Email updated.", string.Empty);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            await _userRepository.Received().UpdateAsync(Arg.Is(targetUser), Arg.Any<CancellationToken>());
            await _memoryCache.Received().RemoveAsync(Arg.Is(cacheKey), Arg.Any<CancellationToken>());

            Assert.Equivalent(expectedResult, result);
            Assert.Equivalent(targetUser.Email, cachedData.NewEmail);
        }
    }
}