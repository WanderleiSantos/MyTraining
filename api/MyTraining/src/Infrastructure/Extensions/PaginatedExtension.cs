using Core.Interfaces.Pagination;
using Infrastructure.Pagination;

namespace Infrastructure.Extensions;

public static class PaginatedExtension
{
    public static async Task<IPaginated<T>> ToPaginatedAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        return await Paginated<T>.CreateAsync(source, pageNumber, pageSize);
    }
}