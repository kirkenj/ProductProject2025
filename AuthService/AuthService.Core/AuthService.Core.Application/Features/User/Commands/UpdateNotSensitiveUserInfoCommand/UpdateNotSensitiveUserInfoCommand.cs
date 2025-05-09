﻿using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand
{
    public class UpdateNotSensitiveUserInfoCommand : IRequest<Response<string>>, IIdObject<Guid>, IUserInfoDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
