﻿using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using CustomResponse;
using HashProvider.Contracts;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserLoginComand
{
    public class UpdateUserLoginComandHandler : IRequestHandler<UpdateUserLoginComand, Response<string>>, IPasswordSettingHandler
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserLoginComandHandler(IUserRepository userRepository, IHashProvider hashProvider)
        {
            _userRepository = userRepository;
            HashPrvider = hashProvider;
        }

        public IHashProvider HashPrvider { get; private set; }

        public async Task<Response<string>> Handle(UpdateUserLoginComand request, CancellationToken cancellationToken)
        {
            var newLogin = request.UpdateUserLoginDto.NewLogin;

            Domain.Models.User? userWithNewlogin = await _userRepository.GetAsync(new UserFilter() { AccurateLogin = newLogin });

            if (userWithNewlogin != null)
            {
                return Response<string>.BadRequestResponse("This login is already taken");
            }

            Domain.Models.User? userToUpdate = await _userRepository.GetAsync(request.UpdateUserLoginDto.Id);

            if (userToUpdate == null)
            {
                return Response<string>.NotFoundResponse(nameof(userToUpdate.Id), true);
            }

            userToUpdate.Login = newLogin;

            await _userRepository.UpdateAsync(userToUpdate);

            return Response<string>.OkResponse("Ok", "Login updated");
        }
    }
}
