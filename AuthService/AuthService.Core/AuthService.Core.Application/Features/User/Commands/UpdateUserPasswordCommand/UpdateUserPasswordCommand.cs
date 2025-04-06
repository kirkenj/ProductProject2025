using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserPasswordCommand
{
    public class UpdateUserPasswordCommand : IRequest<Response<string>>, IPasswordDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = null!;
    }
}
