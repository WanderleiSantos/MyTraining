using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Commands;
using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Models;
using FluentValidation;

namespace Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Validations;

public class InsertSeriesPlanningCommandValidator : AbstractValidator<SeriesPlanningInput>
{
    public InsertSeriesPlanningCommandValidator()
    {
        RuleFor(x => x.SeriesNumber).NotEmpty();
        RuleFor(x => x.Charge).NotEmpty();
        RuleFor(x => x.TrainingSheetSeriesId).NotEmpty();
        RuleFor(x => x.Repetitions).NotEmpty();
        RuleFor(x => x.Interval).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ExercisesIds).NotNull();
    }
}

public class InsertSeriesPlanningCommandServiceValidator : AbstractValidator<InsertSeriesPlanningCommand>
{
    public InsertSeriesPlanningCommandServiceValidator()
    {
        RuleForEach(x => x.SeriesPlanningInputs).SetValidator(new InsertSeriesPlanningCommandValidator());
    }
}