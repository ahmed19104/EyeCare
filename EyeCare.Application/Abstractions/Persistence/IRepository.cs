

namespace EyeCare.Application.Abstractions.Persistence
{
    public interface IRepository<T>
    where T : class
    {
        Task<T?> FirstOrDefaultAsync(
            ISpecification<T> specification,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            ISpecification<T> specification,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(
    ISpecification<T> specification,
    CancellationToken cancellationToken = default);

        Task<PaginatedResult<T>> ListPagedAsync(
    ISpecification<T> specification,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default);
    }
}
