namespace MyTraining.Core.Entities;

public class User : BaseEntity
{
    public User(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Active = true;

        Exercises = new List<Exercise>();
    }
    
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public bool Active { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    public List<Exercise> Exercises { get; private set; }

    public void Update(string firstName, string lastName) 
    {
        base.Update();
        FirstName = firstName;
        LastName = lastName;
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