namespace Core.Interfaces.Pagination;

public interface IPaginatedQuery
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
}