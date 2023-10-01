using FluentValidation;
using MyTraining.Application.UseCases.SearchExerciseById.Commands;

namespace MyTraining.Application.UseCases.SearchExerciseById.Validations;

public class SearchExerciseByIdValidator : AbstractValidator<SearchExerciseByIdCommand>
{
    public SearchExerciseByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}