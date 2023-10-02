using FluentValidation;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;

namespace MyTraining.Application.UseCases.Exercises.UpdateExercise.Validations;

public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}