using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.CreateUserComand
{
    public class CreateUserCommand : IRequest<Response<Guid>>, IEmailUpdateDto, IUserInfoDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
