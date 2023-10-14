namespace Core.Entities;

public class TrainingSheetSeries : BaseEntity
{
    public TrainingSheetSeries(string name, Guid trainingSheetId)
    {
        Name = name;
        Active = true;
        TrainingSheetId = trainingSheetId;
        SeriesPlannings = new List<SeriesPlanning>();
    }

    public string Name { get; private set; }
    public bool Active { get; private set; }
    public Guid TrainingSheetId { get; private set; }
    public TrainingSheet TrainingSheet { get; private set; }
    
    public IEnumerable<SeriesPlanning> SeriesPlannings { get; private set; }

    public void Update(string name)
    {
        base.Update();
        Name = name;
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