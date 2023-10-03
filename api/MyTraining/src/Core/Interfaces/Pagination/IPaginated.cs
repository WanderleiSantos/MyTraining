namespace Core.Interfaces.Pagination;

public interface IPaginated<out T> : IPage
{
    IReadOnlyCollection<T> Items { get; }
}