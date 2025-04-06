using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.UpdateUserLoginCommand;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User.Commands
{
    public class UpdateUserLoginCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly UpdateUserLoginCommandHandler _handler;

        public UpdateUserLoginCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new UpdateUserLoginCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_TargetUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateUserLoginCommand()
            {
                Id = Guid.NewGuid(),
                NewLogin = "SomeLogin"
            };

            _userRepository.GetAsync(request.Id).ReturnsNull();
            var expectedResult = Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_LoginTaken_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateUserLoginCommand()
            {
                Id = Guid.NewGuid(),
                NewLogin = "SomeLogin"
            };

            var targetUser = new Domain.Models.User();

            _userRepository.GetAsync(request.Id).Returns(targetUser);

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateLogin == request.NewLogin))
                .Returns(new Domain.Models.User());

            var expectedResult = Response<string>.BadRequestResponse("This login is already taken");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_LoginFree_UpdatesUserReturnsOk()
        {
            // Arrange
            var request = new UpdateUserLoginCommand()
            {
                Id = Guid.NewGuid(),
                NewLogin = "SomeLogin"
            };

            var targetUser = new Domain.Models.User();

            _userRepository.GetAsync(request.Id).Returns(targetUser);

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateLogin == request.NewLogin))
                .ReturnsNull();

            var expectedResult = Response<string>.OkResponse("Ok", "Login updated");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            await _userRepository.Received()
                .UpdateAsync(Arg.Is(targetUser), Arg.Any<CancellationToken>());
            Assert.Equivalent(expectedResult, result);
            Assert.Equal(targetUser.Login, request.NewLogin);
        }
    }
}