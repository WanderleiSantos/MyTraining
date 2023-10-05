using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;
using FluentValidation;

namespace Application.UseCases.TrainingSheets.InsertTrainingSheet.Validations;

public class InsertTrainingSheetCommandValidator : AbstractValidator<InsertTrainingSheetCommand>
{
    public InsertTrainingSheetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}