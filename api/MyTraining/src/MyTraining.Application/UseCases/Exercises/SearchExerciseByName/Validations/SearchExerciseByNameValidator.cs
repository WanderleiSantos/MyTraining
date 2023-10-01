using FluentValidation;
using MyTraining.Application.UseCases.Exercises.SearchExerciseByName.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchExerciseByName.Validations;

public class SearchExerciseByNameValidator : AbstractValidator<SearchExerciseByNameCommand>
{
    public SearchExerciseByNameValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
    }
}