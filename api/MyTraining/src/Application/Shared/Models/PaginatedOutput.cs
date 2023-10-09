using Core.Interfaces.Pagination;

namespace Application.Shared.Models;

public class PaginatedOutput<T>
{
    private readonly List<T> _items = new();

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

        _items.AddRange(items);
    }

    public int PageCount { get; }

    public int TotalItemCount { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public bool HasPreviousPage { get; }

    public bool HasNextPage { get; }

    public bool IsFirstPage { get; }

    public bool IsLastPage { get; }

    public int FirstItemOnPage { get; }

    public int LastItemOnPage { get; }

    public IReadOnlyCollection<T> Items => _items;
}