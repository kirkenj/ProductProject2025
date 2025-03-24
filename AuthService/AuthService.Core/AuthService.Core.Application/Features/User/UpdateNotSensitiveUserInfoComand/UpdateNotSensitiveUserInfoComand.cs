using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand
{
    public class UpdateNotSensitiveUserInfoComand : IRequest<Response<string>>, IIdObject<Guid>, IUserInfoDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
