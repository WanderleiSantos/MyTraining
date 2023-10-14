namespace Core.Entities;

public class Exercise : BaseEntity
{
    public Exercise(string name, string? link, Guid idUser)
    {
        Name = name;
        Link = link;
        IdUser = idUser;
        Active = true;
    }

    public string Name { get; private set; }
    public string? Link { get; private set; }
    public bool Active { get; private set; }
    
    public Guid IdUser { get; private set; }
    public User? User { get; private set; }    
    
    public List<PlanningExercises> PlanningExercises { get; set; }
    
    public void Update(string name, string? link) 
    {
        base.Update();
        Name = name;
        Link = link;
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