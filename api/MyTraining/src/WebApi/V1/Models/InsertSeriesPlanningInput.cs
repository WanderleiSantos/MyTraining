namespace WebApi.V1.Models;

public class InsertSeriesPlanningInput
{
    public string Machine { get; set; }
    public int SeriesNumber { get; set; }
    public string Repetitions { get; set; }
    public string Charge { get; set; }
    public string Interval { get; set; }
    public List<Guid> ExercisesIds { get; set; }
}