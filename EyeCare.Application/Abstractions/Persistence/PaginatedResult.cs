namespace EyeCare.Application.Abstractions.Persistence;

public sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages =>
        (int)Math.Ceiling(
            TotalCount / (double)PageSize);
}
