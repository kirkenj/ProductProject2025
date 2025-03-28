using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler
{
    public class UpdateUserPasswordComandHandler : IRequestHandler<UpdateUserPasswordComand, Response<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordSetter _passwordSetter;

        public UpdateUserPasswordComandHandler(IUserRepository userRepository, IPasswordSetter passwordSetter)
        {
            _userRepository = userRepository;
            _passwordSetter = passwordSetter;
        }

        public async Task<Response<string>> Handle(UpdateUserPasswordComand request, CancellationToken cancellationToken)
        {
            Domain.Models.User? user = await _userRepository.GetAsync(request.Id);

            if (user == null)
            {
                return Response<string>.NotFoundResponse(nameof(user.Id), true);
            }

            _passwordSetter.SetPassword(request.Password, user);

            await _userRepository.UpdateAsync(user);

            return Response<string>.OkResponse("Ok", "Password updated");
        }
    }
}