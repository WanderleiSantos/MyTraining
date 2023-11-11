using Application.UseCases.SeriesPlannings.UpdateSeriesPlanning.Commands;
using FluentValidation;

namespace Application.UseCases.SeriesPlannings.UpdateSeriesPlanning.Validations;

public class UpdateSeriesPlanningCommandValidator :  AbstractValidator<UpdateSeriesPlanningCommand>
{
    public UpdateSeriesPlanningCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Machine).NotEmpty();
        RuleFor(x => x.SeriesNumber).GreaterThan(0);
        RuleFor(x => x.Repetitions).NotEmpty();
        RuleFor(x => x.Interval).NotEmpty();
        RuleForEach(x => x.ExercisesIds).NotNull();
    }
}