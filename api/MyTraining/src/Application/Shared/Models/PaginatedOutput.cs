using Core.Interfaces.Pagination;

namespace Application.Shared.Models;

public class PaginatedOutput<T>
{
    public PaginatedOutput(IPage paginated, IEnumerable<T> items)
    {
        PageCount = paginated.PageCount;
        TotalItemCount = paginated.TotalItemCount;
        PageNumber = paginated.PageNumber;
        PageSize = paginated.PageSize;
        HasPreviousPage = paginated.HasPreviousPage;
        HasNextPage = paginated.HasNextPage;
        IsFirstPage = paginated.IsFirstPage;
        IsLastPage = paginated.IsLastPage;
        FirstItemOnPage = paginated.FirstItemOnPage;
        LastItemOnPage = paginated.LastItemOnPage;

        Items.AddRange(items);
    }

    public PaginatedOutput()
    {
    }

    public int PageCount { get; set; }
    public int TotalItemCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool IsFirstPage { get; set; }
    public bool IsLastPage { get; set; }
    public int FirstItemOnPage { get; set; }
    public int LastItemOnPage { get; set; }
    public List<T> Items { get; set; } = new();
}