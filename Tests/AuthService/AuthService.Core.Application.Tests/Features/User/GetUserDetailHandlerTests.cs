using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Queries.GetUserDetailQuery;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class GetUserDetailHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetUserDetailQueryHandler _handler;

        public GetUserDetailHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetUserDetailQueryHandler(_userRepository, _mapper);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new GetUserDetailQuery
            {
                Id = Guid.NewGuid(),
            };

            _userRepository.GetAsync(Arg.Is(request.Id)).ReturnsNull();

            var expectedResult = Response<UserDto>.NotFoundResponse(nameof(request.Id), true);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_UserFound_ReturnsMappedValue()
        {
            // Arrange
            var request = new GetUserDetailQuery
            {
                Id = Guid.NewGuid(),
            };

            var targerUser = new Domain.Models.User();

            _userRepository.GetAsync(Arg.Is(request.Id)).Returns(targerUser);

            var targetUserDto = new UserDto();

            _mapper.Map<UserDto>(Arg.Is(targerUser)).Returns(targetUserDto);

            var expectedResult = Response<UserDto>.OkResponse(targetUserDto, "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }
    }
}
