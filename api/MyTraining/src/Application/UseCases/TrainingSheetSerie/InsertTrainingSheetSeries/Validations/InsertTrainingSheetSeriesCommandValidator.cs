using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;
using FluentValidation;

namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Validations;

public class InsertTrainingSheetSeriesCommandValidator : AbstractValidator<InsertTrainingSheetSeriesCommand>
{
    public InsertTrainingSheetSeriesCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.TrainingSheetId).NotEmpty();
    }
}