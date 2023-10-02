using MyTraining.Application.Shared.Models;

namespace MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;

public class SearchAllExercisesCommand : PaginatedInput
{
    public Guid UserId { get; set; }
}