﻿using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserRoleCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Response<string>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
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
