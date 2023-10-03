using Application.Shared.Validators;
using Application.UseCases.Exercises.SearchAllExercises.Commands;
using FluentValidation;

namespace Application.UseCases.Exercises.SearchAllExercises.Validations;

public class SearchAllExercisesValidator : PaginatedInputValidator<SearchAllExercisesCommand>
{
    public SearchAllExercisesValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}