namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Responses;

public class InsertTrainingSheetSeriesResponse
{
    public Guid? Id { get; set; }
    public Guid? TrainingSheetId { get; set; }
    public string? Name { get; set; }
}