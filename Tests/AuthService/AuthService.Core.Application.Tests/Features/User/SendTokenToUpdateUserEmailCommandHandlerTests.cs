using System.Linq.Expressions;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailCommand;
using AuthService.Core.Application.Models.User.Settings;
using Cache.Contracts;
using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class SendTokenToUpdateUserEmailCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducer<ChangeEmailRequest> _changeEmailRequestCreatedProducer;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly UpdateUserEmailSettings _updateUserEmailSettings;
        private readonly SendTokenToUpdateUserEmailCommandHandler _handler;

        public SendTokenToUpdateUserEmailCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _changeEmailRequestCreatedProducer = Substitute.For<IKafkaProducer<ChangeEmailRequest>>();
            _passwordGenerator = Substitute.For<IPasswordGenerator>();
            _memoryCache = Substitute.For<ICustomMemoryCache>();
            _updateUserEmailSettings = new()
            {
                EmailUpdateTimeOutHours = 1,
                UpdateUserEmailCacheKeyFormat = "asf {0}"
            };

            _handler = new SendTokenToUpdateUserEmailCommandHandler(
                _userRepository,
                _memoryCache,
                _passwordGenerator,
                Options.Create(_updateUserEmailSettings),
                _changeEmailRequestCreatedProducer);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new SendTokenToUpdateUserEmailRequest
            {
                Email = "SomeEmail",
                Id = Guid.NewGuid(),
            };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_RequestValid_SendsNotificationCachesToken()
        {
            // Arrange
            var request = new SendTokenToUpdateUserEmailRequest
            {
                Email = "SomeEmail",
                Id = Guid.NewGuid(),
            };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(new Domain.Models.User());

            var tokens = new string[] { "token1", "token2" };

            _passwordGenerator.Generate().Returns(tokens[0], tokens[1]);

            Expression<Predicate<ChangeEmailRequest>> comparePredicate = r => r.OtpToNewEmail == tokens[0]
                                && r.UserId == request.Id
                                && r.NewEmail == request.Email
                                && r.OtpToOldEmail == tokens[1];

            var expectedResult = Response<string>.OkResponse("Check emails to get further details", string.Empty);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);

            await _memoryCache.Received().SetAsync(
                Arg.Is(string.Format(_updateUserEmailSettings.UpdateUserEmailCacheKeyFormat, request.Id)),
                Arg.Is<ChangeEmailRequest>(comparePredicate),
                Arg.Is(TimeSpan.FromHours(_updateUserEmailSettings.EmailUpdateTimeOutHours)),
                Arg.Any<CancellationToken>());

            await _changeEmailRequestCreatedProducer.ProduceAsync(
                Arg.Is<ChangeEmailRequest>(comparePredicate),
                Arg.Any<CancellationToken>());
        }
    }
}