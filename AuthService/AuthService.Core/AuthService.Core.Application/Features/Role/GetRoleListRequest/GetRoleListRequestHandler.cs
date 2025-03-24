using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.Role.DTOs;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleListRequest
{
    public class GetRoleListRequestHandler : IRequestHandler<GetRoleListRequest, Response<List<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRoleListRequestHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<RoleDto>>> Handle(GetRoleListRequest request, CancellationToken cancellationToken)
        {
            var users = await _roleRepository.GetAllAsync();
            return Response<List<RoleDto>>.OkResponse(_mapper.Map<List<RoleDto>>(users), "Success");
        }
    }
}
