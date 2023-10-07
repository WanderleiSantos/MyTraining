using Application.Shared.Extensions;
using Application.UseCases.Users.ChangeUserPassword.Commands;
using FluentValidation;

namespace Application.UseCases.Users.ChangeUserPassword.Validations;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.OldPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Must(x => x.IsPassWordValid())
            .WithMessage("'New Password' must contain at least 8 characters, one number, one uppercase letter, one lowercase letter and one special character.");
    }
}