using FluentValidation;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;

namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises.Validations;

public class SearchAllExercisesValidator : AbstractValidator<SearchAllExercisesCommand>
{
    public SearchAllExercisesValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}