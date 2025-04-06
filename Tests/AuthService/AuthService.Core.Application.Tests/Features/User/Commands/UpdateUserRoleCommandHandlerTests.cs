using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.UpdateUserRoleCommand;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User.Commands
{
    public class UpdateUserRoleCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UpdateUserRoleCommandHandler _handler;

        public UpdateUserRoleCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _handler = new UpdateUserRoleCommandHandler(_userRepository, _roleRepository);
        }

        [Fact]
        public async Task Handle_RoleNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateUserRoleCommand
            {
                Id = Guid.NewGuid(),
                RoleID = 3
            };

            _roleRepository.GetAsync(Arg.Is(request.RoleID), Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse(nameof(Domain.Models.Role), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);
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

            _roleRepository.GetAsync(Arg.Is(request.RoleID), Arg.Any<CancellationToken>())
                .Returns(new Domain.Models.Role());

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

            _roleRepository.GetAsync(Arg.Is(request.RoleID), Arg.Any<CancellationToken>())
                .Returns(new Domain.Models.Role());

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
