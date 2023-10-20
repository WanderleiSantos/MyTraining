using Application.UseCases.Exercises.SearchExerciseById.Commands;
using FluentValidation;

namespace Application.UseCases.Exercises.SearchExerciseById.Validations;

public class SearchExerciseByIdValidator : AbstractValidator<SearchExerciseByIdCommand>
{
    public SearchExerciseByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}