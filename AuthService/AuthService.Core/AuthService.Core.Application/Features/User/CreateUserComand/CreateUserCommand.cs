using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.CreateUserComand
{
    public class CreateUserCommand : IRequest<Response<Guid>>
    {
        public CreateUserDto CreateUserDto { get; set; } = null!;
    }
}
