using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserLoginCommand
{
    public class UpdateUserLoginCommandHandler : IRequestHandler<UpdateUserLoginCommand, Response<string>>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserLoginCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Response<string>> Handle(UpdateUserLoginCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? userToUpdate = await _userRepository.GetAsync(request.Id);
            if (userToUpdate == null)
            {
                return Response<string>.NotFoundResponse(nameof(Domain.Models.User), true);
            }

            var newLogin = request.NewLogin;

            Domain.Models.User? userWithNewlogin = await _userRepository.GetAsync(new UserFilter() { AccurateLogin = newLogin });
            if (userWithNewlogin != null)
            {
                return Response<string>.BadRequestResponse("This login is already taken");
            }

            userToUpdate.Login = newLogin;

            await _userRepository.UpdateAsync(userToUpdate, cancellationToken);

            return Response<string>.OkResponse("Ok", "Login updated");
        }
    }
}