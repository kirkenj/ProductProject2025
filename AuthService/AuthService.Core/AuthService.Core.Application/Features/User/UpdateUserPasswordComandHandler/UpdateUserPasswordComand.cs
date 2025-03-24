using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler
{
    public class UpdateUserPasswordComand : IRequest<Response<string>>, IPasswordDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = null!;
    }
}
