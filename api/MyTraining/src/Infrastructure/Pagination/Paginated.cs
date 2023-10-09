using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Pagination;

namespace Infrastructure.Pagination;

public class Paginated<T> : IPaginated<T>
{
    private readonly List<T> _items = new();
    
    private Paginated(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalItemCount = count;
        PageSize = pageSize;
        PageNumber = pageNumber;
        PageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;
        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < PageCount;
        IsFirstPage = PageNumber == 1;
        IsLastPage = PageNumber >= PageCount;
        FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
        var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
        LastItemOnPage = numberOfLastItemOnPage > TotalItemCount ? TotalItemCount : numberOfLastItemOnPage;

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
    
    public static async Task<IPaginated<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new Paginated<T>(items, count, pageNumber, pageSize);
    }
}