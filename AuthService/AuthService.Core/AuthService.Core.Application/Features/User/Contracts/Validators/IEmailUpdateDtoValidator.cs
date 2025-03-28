﻿using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Interfaces.Validators
{
    public class IEmailUpdateDtoValidator : AbstractValidator<IEmailUpdateDto>
    {
        public IEmailUpdateDtoValidator(IUserRepository userRepository)
        {
            Include(new IEmailDtoValidator());

            RuleFor(p => p.Email)
                .MustAsync(async (Email, cancellationToken) =>
                {
                    var resultUser = await userRepository.GetAsync(new UserFilter { AccurateEmail = Email });
                    if (resultUser == null)
                    {
                        return true;
                    }

                    return resultUser.Email != Email;
                })
                .WithMessage("{PropertyName} is taken");
        }
    }
}
