using Application.Shared.Models;

namespace WebApi.V1.Queries;

public class SearchAllExercises : SortedPaginatedInput
{
    public string? Name { get; set; }
}