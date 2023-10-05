namespace Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;

public class InsertTrainingSheetCommand
{
    public string Name { get; set; }
    public string? TimeExchange { get; set; }
    public Guid UserId { get; set; }
}