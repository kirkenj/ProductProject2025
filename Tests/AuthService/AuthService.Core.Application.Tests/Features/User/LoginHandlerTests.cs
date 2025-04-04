using System.Text;
using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Login;
using AuthService.Core.Application.Models.DTOs.User;
using AutoMapper;
using CustomResponse;
using HashProvider.Contracts;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.User
{
    public class LoginHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHashProvider _hashProvider;
        private readonly IMapper _mapper;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _roleRepository = Substitute.For<IRoleRepository>();
            _hashProvider = Substitute.For<IHashProvider>();
            _mapper = Substitute.For<IMapper>();
            _handler = new LoginHandler(_userRepository, _roleRepository, _hashProvider, _mapper);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == request.Email)).ReturnsNull();

            var expectedResult = Response<UserDto>.BadRequestResponse("Wrong password or email");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            var targetUser = new Domain.Models.User
            {
                HashAlgorithm = "SomeAlgorithm",
                StringEncoding = Encoding.UTF8.BodyName,
                PasswordHash = Guid.NewGuid().ToString(),
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == request.Email))
                .Returns(targetUser);

            _hashProvider.GetHash(Arg.Is(request.Password)).Returns(Guid.NewGuid().ToString());

            var expectedResult = Response<UserDto>.BadRequestResponse("Wrong password or email");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equal(targetUser.HashAlgorithm, _hashProvider.HashAlgorithmName);
            Assert.Equal(targetUser.StringEncoding, _hashProvider.Encoding.BodyName);
            Assert.Equivalent(expectedResult, result);
        }

        [Fact]
        public async Task Handle_ValidPassword_ReturnsUserDto()
        {
            // Arrange
            var request = new LoginRequest()
            {
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            var targetUser = new Domain.Models.User
            {
                HashAlgorithm = "SomeAlgorithm",
                StringEncoding = Encoding.UTF8.BodyName,
                PasswordHash = Guid.NewGuid().ToString(),
            };

            _userRepository.GetAsync(Arg.Is<UserFilter>(f => f.AccurateEmail == request.Email))
                .Returns(targetUser);

            _hashProvider.GetHash(Arg.Is(request.Password)).Returns(targetUser.PasswordHash);

            var targetUserDto = new UserDto();

            _mapper.Map<UserDto>(Arg.Is(targetUser)).Returns(targetUserDto);

            var expectedResult = Response<UserDto>.OkResponse(targetUserDto, "Success");

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Equal(targetUser.HashAlgorithm, _hashProvider.HashAlgorithmName);
            Assert.Equal(targetUser.StringEncoding, _hashProvider.Encoding.BodyName);
            Assert.Equivalent(expectedResult, result);
            Assert.Equal(expectedResult.Result, result.Result);
        }
    }
}