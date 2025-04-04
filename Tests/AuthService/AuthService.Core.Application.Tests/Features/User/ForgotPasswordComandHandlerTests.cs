using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.ForgotPasswordComand;
using CustomResponse;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class ForgotPasswordComandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducer<ForgotPassword> _producer;
        private readonly IPasswordSetter _passwordSetter;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ForgotPasswordComandHandler _handler;

        public ForgotPasswordComandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _producer = Substitute.For<IKafkaProducer<ForgotPassword>>();
            _passwordSetter = Substitute.For<IPasswordSetter>();
            _passwordGenerator = Substitute.For<IPasswordGenerator>();
            _handler = new ForgotPasswordComandHandler(_userRepository, _producer, _passwordSetter, _passwordGenerator);
        }

        [Fact]
        public async Task HandleAsync_UserNotFound_ReturnsOkNotGeneratesAPassword()
        {
            // Arrange
            var request = new ForgotPasswordComand
            {
                Email = Guid.NewGuid().ToString(),
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(u => u.AccurateEmail == request.Email))
                .ReturnsNull();

            var expectedResult = Response<string>.OkResponse("New password was sent on your email", "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            _passwordGenerator.DidNotReceive().Generate();
            Assert.Equivalent(result, expectedResult);
        }

        [Fact]
        public async Task HandleAsync_UserFound_ReturnsOkUpdatesPassword()
        {
            // Arrange
            var request = new ForgotPasswordComand
            {
                Email = Guid.NewGuid().ToString(),
            };

            var targetUser = new Domain.Models.User();

            _userRepository.GetAsync(Arg.Is<UserFilter>(u => u.AccurateEmail == request.Email))
                .Returns(targetUser);

            var newPassword = Guid.NewGuid().ToString();

            _passwordGenerator.Generate().Returns(newPassword);

            var expectedResult = Response<string>.OkResponse("New password was sent on your email", "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);

            _passwordSetter.Received().SetPassword(Arg.Is(newPassword), Arg.Is(targetUser));
            await _userRepository.Received().UpdateAsync(Arg.Is(targetUser));

            await _producer.Received().ProduceAsync(Arg.Is<ForgotPassword>(fp => fp.UserId == targetUser.Id && fp.NewPassword == newPassword), Arg.Any<CancellationToken>());
        }
    }
}
