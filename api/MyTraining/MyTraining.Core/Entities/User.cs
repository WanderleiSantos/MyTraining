namespace MyTraining.Core.Entities;

public class User : BaseEntity
{
    public User(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
        Active = true;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool Active { get; private set; }
}