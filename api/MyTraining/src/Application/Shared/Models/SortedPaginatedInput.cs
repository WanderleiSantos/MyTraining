using AspNetCore.IQueryable.Extensions.Sort;

namespace Application.Shared.Models;

public abstract class SortedPaginatedInput : PaginatedInput, IQuerySort
{
    public string? Sort { get; set; }
}