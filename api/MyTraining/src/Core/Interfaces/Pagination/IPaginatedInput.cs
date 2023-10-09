namespace Core.Interfaces.Pagination;

public interface IPaginatedInput
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
}