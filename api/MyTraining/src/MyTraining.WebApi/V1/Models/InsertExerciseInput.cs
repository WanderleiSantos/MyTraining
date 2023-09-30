namespace MyTraining.API.V1.Models;

public class InsertExerciseInput
{
    public string Name { get; set; }
    public string Link { get; set; }
    public Guid UserId { get; set; }
}