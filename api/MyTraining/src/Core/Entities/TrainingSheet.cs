namespace Core.Entities;

public class TrainingSheet : BaseEntity
{
    public TrainingSheet(string name, string? timeExchange, Guid userId)
    {
        Name = name;
        TimeExchange = timeExchange;
        UserId = userId;
        Active = true;
        TrainingSheetSeries = new List<TrainingSheetSeries>();
    }

    public string Name { get; private set; }
    public string? TimeExchange { get; private set; }
    public bool Active { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    
    public ICollection<TrainingSheetSeries> TrainingSheetSeries { get; private set; }

    public void Update(string name, string timeExchange)
    {
        base.Update();
        Name = name;
        TimeExchange = timeExchange;
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