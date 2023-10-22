namespace Application.UseCases.TrainingSheetSerie.InsertTrainingSheetSeries.Commands;

public class InsertTrainingSheetSeriesCommand
{
    public Guid TrainingSheetId { get;  set; }
    public string Name { get; set; }
}