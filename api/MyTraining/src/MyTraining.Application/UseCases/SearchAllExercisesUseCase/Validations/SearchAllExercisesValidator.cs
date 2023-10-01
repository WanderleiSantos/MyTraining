using FluentValidation;
using MyTraining.Application.UseCases.SearchAllExercisesUseCase.Commands;

namespace MyTraining.Application.UseCases.SearchAllExercisesUseCase.Validations;

public class SearchAllExercisesValidator : AbstractValidator<SearchAllExercisesCommand>
{
    public SearchAllExercisesValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}