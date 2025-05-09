﻿using FluentValidation;

namespace NotificationService.Core.Application.Features.AuthService.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(c => c.NewPassword).NotEmpty();
            RuleFor(c => c.UserId).Must(id => id != default);
        }
    }
}
