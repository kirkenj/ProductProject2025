using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.DTOs.Role;
using AutoMapper;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.Queries.GetRoleListRequest
{
    public class GetRoleListQueryHandler : IRequestHandler<GetRoleListQuery, Response<List<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRoleListQueryHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Response<List<RoleDto>>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
        {
            var users = await _roleRepository.GetAllAsync(cancellationToken);
            return Response<List<RoleDto>>.OkResponse(_mapper.Map<List<RoleDto>>(users), "Success");
        }
    }
}