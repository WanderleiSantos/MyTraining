using FluentValidation;
using MyTraining.Application.UseCases.SearchExercisesByUserUseCase.Commands;

namespace MyTraining.Application.UseCases.SearchExercisesByUserUseCase.Validations;

public class SearchExerciseByUserValidator : AbstractValidator<SearchExerciseByUserCommand>
{
    public SearchExerciseByUserValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}