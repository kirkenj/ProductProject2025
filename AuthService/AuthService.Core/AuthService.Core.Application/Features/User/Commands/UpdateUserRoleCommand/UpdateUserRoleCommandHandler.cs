using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UpdateUserRoleCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Response<string>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleRepository.GetAsync(request.RoleID, cancellationToken) == null)
            {
                return Response<string>.NotFoundResponse(nameof(Domain.Models.Role), true);
            }

            Domain.Models.User? user = await _userRepository.GetAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);
            }

            user.RoleID = request.RoleID;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Response<string>.OkResponse("Ok", "Role updated");
        }
    }
}
