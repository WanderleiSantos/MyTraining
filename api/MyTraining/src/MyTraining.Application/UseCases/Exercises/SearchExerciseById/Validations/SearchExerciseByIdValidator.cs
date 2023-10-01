using FluentValidation;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchExerciseById.Validations;

public class SearchExerciseByIdValidator : AbstractValidator<SearchExerciseByIdCommand>
{
    public SearchExerciseByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}