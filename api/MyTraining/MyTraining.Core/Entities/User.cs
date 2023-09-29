namespace MyTraining.Core.Entities;

public class User : BaseEntity
{
    public User(string name, string lastName, string email, string userName, string password)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        UserName = userName;
        Password = password;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
        Active = true;
    }

    public string Name { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool Active { get; private set; }
}