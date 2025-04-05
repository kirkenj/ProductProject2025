using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.UpdateUserRoleCommand;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class UpdateUserRoleCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly UpdateUserRoleCommandHandler _handler;

        public UpdateUserRoleCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new UpdateUserRoleCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateUserRoleCommand
            {
                Id = Guid.NewGuid(),
                RoleID = 3
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
        public async Task Handle_UserFound_UpdatesRoleReturnsOk()
        {
            // Arrange
            var request = new UpdateUserRoleCommand
            {
                Id = Guid.NewGuid(),
                RoleID = 3
            };

            var targetUser = new Domain.Models.User { };

            _userRepository.GetAsync(Arg.Is(request.Id), Arg.Any<CancellationToken>())
                .Returns(targetUser);

            var expectedResult = Response<string>.OkResponse("Ok", "Role updated");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            await _userRepository.Received().UpdateAsync(Arg.Is(targetUser), Arg.Any<CancellationToken>());
            Assert.Equivalent(result, expectedResult);
        }
    }
}
