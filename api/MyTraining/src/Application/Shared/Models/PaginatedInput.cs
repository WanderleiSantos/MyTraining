using Core.Interfaces.Pagination;

namespace Application.Shared.Models;

public abstract class PaginatedInput : IPaginatedInput
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}