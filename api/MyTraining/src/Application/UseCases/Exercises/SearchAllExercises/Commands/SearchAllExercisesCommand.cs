using Application.Shared.Models;

namespace Application.UseCases.Exercises.SearchAllExercises.Commands;

public class SearchAllExercisesCommand : PaginatedInput
{
    public Guid UserId { get; set; }
}