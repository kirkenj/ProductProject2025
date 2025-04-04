﻿using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.Role.GetRoleDetail;
using AuthService.Core.Application.Models.DTOs.Role;
using AutoMapper;
using CustomResponse;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuthService.Core.Application.Tests.Features.Role
{
    public class GetRoleDetailHanlerTests
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly GetRoleDetailHandler _hanler;

        public GetRoleDetailHanlerTests()
        {
            _roleRepository = Substitute.For<IRoleRepository>();
            _mapper = Substitute.For<IMapper>();
            _hanler = new GetRoleDetailHandler(_roleRepository, _mapper);
        }

        [Fact]
        public async Task Handle_IdNotFound_ReturnsOkResponse()
        {
            // Arrange
            var request = new GetRoleDtoRequest()
            {
                Id = 1
            };

            _roleRepository.GetAsync(Arg.Is(request.Id)).ReturnsNull();

            var expectedResult = Response<RoleDto>.NotFoundResponse(nameof(request.Id), true);

            // Act
            var result = await _hanler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);
        }

        [Fact]
        public async Task Handle_IdFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var request = new GetRoleDtoRequest()
            {
                Id = 1
            };

            var roleToReturn = new Domain.Models.Role
            {
                Id = request.Id,
                Name = "ARole"
            };

            var roleDto = new RoleDto { };

            _roleRepository.GetAsync(Arg.Is(request.Id)).Returns(roleToReturn);

            _mapper.Map<RoleDto>(Arg.Is(roleToReturn)).Returns(roleDto);

            var expectedResult = Response<RoleDto>.OkResponse(roleDto, string.Empty);

            // Act
            var result = await _hanler.Handle(request, default);

            // Assert
            Assert.Equivalent(result, expectedResult);
        }
    }
}
