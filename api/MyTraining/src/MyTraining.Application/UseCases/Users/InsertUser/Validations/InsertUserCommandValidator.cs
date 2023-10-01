using FluentValidation;
using MyTraining.Application.Shared.Extensions;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;

namespace MyTraining.Application.UseCases.Users.InsertUser.Validations;

public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
{
    public InsertUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(x => x.IsPassWordValid())
            .WithMessage("Password must contain at least 8 characters, one number, one uppercase letter, one lowercase letter and one special character");
    }
}