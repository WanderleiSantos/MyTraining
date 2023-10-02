using MyTraining.Core.Interfaces.Pagination;
using MyTraining.Infrastructure.Pagination;

namespace MyTraining.Infrastructure.Extensions;

public static class PaginatedExtension
{
    public static async Task<IPaginated<T>> ToPaginatedAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        return await Paginated<T>.CreateAsync(source, pageNumber, pageSize);
    }
}