using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.GetUserList;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using NSubstitute;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class GetUserListHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetUserListHandler _handler;

        public GetUserListHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetUserListHandler(_userRepository, _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsOkAndUserDtoList()
        {
            // Arrange
            var request = new GetUserListRequest
            {
                Page = 1,
                PageSize = 10,
                UserFilter = new()
            };

            var userList = new List<Domain.Models.User>();
            var userDtoList = new List<UserDto>();

            _userRepository.GetPageContentAsync(
                Arg.Is(request.UserFilter),
                Arg.Is(request.Page),
                Arg.Is(request.PageSize),
                Arg.Any<CancellationToken>())
                .Returns(userList);

            _mapper.Map<List<UserDto>>(Arg.Is(userList)).Returns(userDtoList);

            var expectedResult = Response<List<UserDto>>.OkResponse(userDtoList, "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
            Assert.Equal(expectedResult.Result, result.Result);
        }
    }
}