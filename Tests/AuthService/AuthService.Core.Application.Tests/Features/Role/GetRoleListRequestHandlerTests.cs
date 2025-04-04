using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.Role.GetRoleListRequest;
using AuthService.Core.Application.Models.DTOs.Role;
using AutoMapper;
using CustomResponse;
using NSubstitute;

namespace AuthService.Core.Application.Tests.Features.Role
{
    public class GetRoleListRequestHandlerTests
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly GetRoleListRequestHandler _hanler;

        public GetRoleListRequestHandlerTests()
        {
            _roleRepository = Substitute.For<IRoleRepository>();
            _mapper = Substitute.For<IMapper>();
            _hanler = new GetRoleListRequestHandler(_roleRepository, _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsMappedValuesWithOkResponse()
        {
            // Arrange
            var request = new GetRoleListRequest();

            var roles = new List<Domain.Models.Role>() { new() };

            _roleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(roles);

            var roleDtoList = new List<RoleDto>();

            _mapper.Map<List<RoleDto>>(Arg.Is(roles)).Returns(roleDtoList);

            var expectedResult = Response<List<RoleDto>>.OkResponse(roleDtoList, "Success");

            // Act
            var result = await _hanler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);
        }
    }
}