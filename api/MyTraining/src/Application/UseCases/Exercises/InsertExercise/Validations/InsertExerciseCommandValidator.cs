using Application.UseCases.Exercises.InsertExercise.Commands;
using FluentValidation;

namespace Application.UseCases.Exercises.InsertExercise.Validations;

public class InsertExerciseCommandValidator : AbstractValidator<InsertExerciseCommand>
{
    public InsertExerciseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}