using Core.Entities;

namespace Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Models;

public class SeriesPlanningInput
{
    public string Machine { get; set; }
    public int SeriesNumber { get; set; }
    public string Repetitions { get; set; }
    public string Charge { get; set; }
    public string Interval { get; set; }
    public Guid TrainingSheetSeriesId { get; set; }
    public Guid UserId { get; set; }
    public List<Guid> ExercisesIds { get; set; }
}