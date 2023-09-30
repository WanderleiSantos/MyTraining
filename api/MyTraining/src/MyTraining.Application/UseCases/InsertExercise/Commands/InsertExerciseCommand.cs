namespace MyTraining.Application.UseCases.InsertExercise.Commands;

public class InsertExerciseCommand
{
    public string Name { get; set; }
    public string Link { get; set; }
    public Guid UserId { get; set; }
}