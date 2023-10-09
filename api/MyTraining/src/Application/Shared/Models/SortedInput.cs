using AspNetCore.IQueryable.Extensions.Sort;

namespace Application.Shared.Models;

public abstract class SortedInput : IQuerySort
{
    public string? Sort { get; set; }
}