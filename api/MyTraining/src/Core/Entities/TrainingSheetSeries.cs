namespace Core.Entities;

public class TrainingSheetSeries : BaseEntity
{
    public TrainingSheetSeries(string name)
    {
        Name = name;
        Active = true;
    }

    public string Name { get; private set; }
    public bool Active { get; private set; }
    public Guid TrainingSheetId { get; private set; }
    public TrainingSheet TrainingSheet { get; private set; }

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