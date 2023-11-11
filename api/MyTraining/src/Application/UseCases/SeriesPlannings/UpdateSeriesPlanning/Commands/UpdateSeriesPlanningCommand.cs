namespace Application.UseCases.SeriesPlannings.UpdateSeriesPlanning.Commands;

public class UpdateSeriesPlanningCommand
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid UserId { get; set; }
    public string Machine { get; set; }
    public int SeriesNumber { get; set; }
    public string Repetitions { get; set; }
    public string Charge { get; set; }
    public string Interval { get; set; }
    public List<Guid> ExercisesIds { get; set; }
}