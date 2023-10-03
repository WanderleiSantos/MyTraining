using Application.UseCases.Auth.SignIn.Commands;
using FluentValidation;

namespace Application.UseCases.Auth.SignIn.Validations;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}