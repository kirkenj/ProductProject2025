﻿using AuthService.Core.Application.Features.User.DTOs;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.GetUserDto
{
    public class GetUserDtoRequest : IRequest<Response<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
