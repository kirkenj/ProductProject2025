using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand;
using AutoMapper;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class UpdateNotSensitiveUserInfoCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UpdateNotSensitiveUserInfoCommandHandler _handler;

        public UpdateNotSensitiveUserInfoCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new UpdateNotSensitiveUserInfoCommandHandler(_userRepository, _mapper);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdateNotSensitiveUserInfoCommand { Id = Guid.NewGuid(), Address = "SomeAddress", Name = "SomeName" };
            _userRepository.GetAsync(request.Id, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var expectedResult = Response<string>.NotFoundResponse(nameof(request.Id), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_UserFound_UpdatesUserReturnsOk()
        {
            // Arrange
            var request = new UpdateNotSensitiveUserInfoCommand { Id = Guid.NewGuid(), Address = "SomeAddress", Name = "SomeName" };

            var targetUser = new Domain.Models.User();

            _userRepository.GetAsync(request.Id, Arg.Any<CancellationToken>())
                .Returns(targetUser);

            var expectedResult = Response<string>.OkResponse("Ok", "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            _mapper.Received().Map(request, targetUser);

            await _userRepository.Received().UpdateAsync(targetUser, Arg.Any<CancellationToken>());

            Assert.Equivalent(expectedResult, result);
        }
    }
}