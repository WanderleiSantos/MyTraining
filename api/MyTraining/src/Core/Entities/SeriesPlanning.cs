namespace Core.Entities;

public class SeriesPlanning : BaseEntity
{
    public SeriesPlanning(string machine, int seriesNumber, string repetitions, string charge, string interval,
        Guid trainingSheetSeriesId)
    {
        Machine = machine;
        SeriesNumber = seriesNumber;
        Repetitions = repetitions;
        Charge = charge;
        Interval = interval;
        Active = true;
        TrainingSheetSeriesId = trainingSheetSeriesId;
        PlanningExercises = new List<PlanningExercises>();
    }

    public string Machine { get; private set; }
    public int SeriesNumber { get; private set; }
    public string Repetitions { get; private set; }
    public string Charge { get; private set; }
    public string Interval { get; private set; }
    public bool Active { get; private set; }
    public Guid TrainingSheetSeriesId { get; private set; }
    public TrainingSheetSeries TrainingSheetSeries { get; private set; }
    public List<PlanningExercises> PlanningExercises { get; private set; }

    public void Update(string machine, int seriesNumber, string repetitions, string charge, string interval)
    {
        base.Update();
        Machine = machine;
        SeriesNumber = seriesNumber;
        Repetitions = repetitions;
        Charge = charge;
        Interval = interval;
    }

    public void Activate()
    {
        base.Update();
        Active = true;
    }

    public void Deactivate()
    {
        base.Update();
        Active = false;
    }
}