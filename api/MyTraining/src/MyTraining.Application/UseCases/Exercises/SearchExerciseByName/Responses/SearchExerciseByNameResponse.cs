namespace MyTraining.Application.UseCases.Exercises.SearchExerciseByName.Responses;

public class SearchExerciseByNameResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Link { get; set; }
    public bool? Active { get; set; }
}