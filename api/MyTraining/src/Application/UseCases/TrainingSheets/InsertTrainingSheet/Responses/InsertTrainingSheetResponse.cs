namespace Application.UseCases.TrainingSheets.InsertTrainingSheet.Responses;

public class InsertTrainingSheetResponse
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? TimeExchange { get; set; }
    public bool? Active { get; set; }
}