using MyTraining.Core.Interfaces.Pagination;

namespace MyTraining.Application.Shared.Models;

public abstract class PaginatedInput : IPaginatedQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}