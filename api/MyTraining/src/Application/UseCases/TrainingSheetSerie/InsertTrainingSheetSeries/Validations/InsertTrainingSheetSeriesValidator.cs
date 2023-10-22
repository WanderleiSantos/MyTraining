using Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;
using FluentValidation;

namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Validations;

public class InsertTrainingSheetSeriesValidator : AbstractValidator<InsertTrainingSheetSeriesCommand>
{
    public InsertTrainingSheetSeriesValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.TrainingSheetId).NotEmpty();
    }
}