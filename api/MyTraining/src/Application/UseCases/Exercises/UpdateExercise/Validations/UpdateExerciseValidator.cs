using Application.UseCases.Exercises.UpdateExercise.Commands;
using FluentValidation;

namespace Application.UseCases.Exercises.UpdateExercise.Validations;

public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}