using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.UpdateUserPasswordCommand;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User.Commands
{
    public class UpdateUserPasswordCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordSetter _passwordSetter;
        private readonly UpdateUserPasswordCommandHandler _handler;

        public UpdateUserPasswordCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordSetter = Substitute.For<IPasswordSetter>();
            _handler = new UpdateUserPasswordCommandHandler(_userRepository, _passwordSetter);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateUserPasswordCommand
            {
                Id = Guid.NewGuid(),
                Password = "SomePassword"
            };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);
        }

        [Fact]
        public async Task Handle_UserFound_UpdatesPasswordReturnsOk()
        {
            // Arrange
            var request = new UpdateUserPasswordCommand
            {
                Id = Guid.NewGuid(),
                Password = "SomePassword"
            };

            var targetUser = new Domain.Models.User { };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targetUser);

            var expectedResult = Response<string>.OkResponse("Ok", "Password updated");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            _passwordSetter.Received().SetPassword(Arg.Is(request.Password), Arg.Is(targetUser));
            await _userRepository.Received().UpdateAsync(Arg.Is(targetUser), Arg.Any<CancellationToken>());
            Assert.Equivalent(result, expectedResult);
        }
    }
}
