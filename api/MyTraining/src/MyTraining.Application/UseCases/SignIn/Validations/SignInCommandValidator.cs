using FluentValidation;
using MyTraining.Application.UseCases.SignIn.Commands;

namespace MyTraining.Application.UseCases.SignIn.Validations;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}