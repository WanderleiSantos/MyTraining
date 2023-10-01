using FluentValidation;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Commands;

namespace MyTraining.Application.UseCases.Exercises.InsertExercise.Validations;

public class InsertExerciseCommandValidator : AbstractValidator<InsertExerciseCommand>
{
    public InsertExerciseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}