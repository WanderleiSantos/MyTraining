using Application.Shared.Models;
using AspNetCore.IQueryable.Extensions.Attributes;
using AspNetCore.IQueryable.Extensions.Filter;

namespace Application.UseCases.Exercises.SearchAllExercises.Commands;

public class SearchAllExercisesCommand : SortedPaginatedInput
{
    public Guid UserId { get; set; }
    
    [QueryOperator(Operator = WhereOperator.Contains, CaseSensitive = false)]
    public string? Name { get; set; }
}