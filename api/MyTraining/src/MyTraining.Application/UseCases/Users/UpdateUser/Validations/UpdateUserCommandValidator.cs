using FluentValidation;
using MyTraining.Application.UseCases.Users.UpdateUser.Commands;

namespace MyTraining.Application.UseCases.Users.UpdateUser.Validations;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(3);
    }
}