using FluentValidation;
using MyTraining.Application.Shared.Validators;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises.Validations;

public class SearchAllExercisesValidator : PaginatedInputValidator<SearchAllExercisesCommand>
{
    public SearchAllExercisesValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}