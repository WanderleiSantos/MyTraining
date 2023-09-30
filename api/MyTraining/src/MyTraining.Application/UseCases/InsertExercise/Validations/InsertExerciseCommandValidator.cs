using FluentValidation;
using MyTraining.Application.UseCases.InsertExercise.Commands;

namespace MyTraining.Application.UseCases.InsertExercise.Validations;

public class InsertExerciseCommandValidator : AbstractValidator<InsertExerciseCommand>
{
    public InsertExerciseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}