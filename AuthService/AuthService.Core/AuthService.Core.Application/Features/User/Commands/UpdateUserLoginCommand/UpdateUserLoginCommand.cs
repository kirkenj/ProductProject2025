using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserLoginCommand
{
    public class UpdateUserLoginCommand : IRequest<Response<string>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string NewLogin { get; set; } = null!;
    }
}
