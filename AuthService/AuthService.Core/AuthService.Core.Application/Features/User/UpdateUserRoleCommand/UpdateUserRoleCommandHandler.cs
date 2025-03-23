using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Response<string>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public UpdateUserRoleCommandHandler(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<Response<string>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.UpdateUserRoleDTO.Id);

            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(user.Email), true);
            }

            user.RoleID = request.UpdateUserRoleDTO.RoleID;

            await _userRepository.UpdateAsync(user);

            return Response<string>.OkResponse("Ok", "Role updated");
        }
    }
}
