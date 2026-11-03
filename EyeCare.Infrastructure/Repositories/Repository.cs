using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        private IQueryable<T> ApplySpecification(
     ISpecification<T> specification)
        {
            IQueryable<T> query =
                _context.Set<T>();

            if (specification.Criteria is not null)
            {
                query = query.Where(
                    specification.Criteria);
            }

            if (specification.Includes != null &&
                specification.Includes.Any())
            {
                foreach (var include in specification.Includes)
                {
                    query = query.Include(include);
                }
            }

           

            if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(
                    specification.OrderByDescending);
            }

            return query;
        }
        public async Task<T?> FirstOrDefaultAsync(
     ISpecification<T> specification,
     CancellationToken cancellationToken = default)
        {
            var query =
                ApplySpecification(specification).AsNoTracking();

            return await query.FirstOrDefaultAsync(
                cancellationToken);
        }

        public async Task<bool> AnyAsync(
    ISpecification<T> specification,
    CancellationToken cancellationToken = default)
        {
            var query =
                ApplySpecification(specification);

            return await query.AnyAsync(
                cancellationToken);
        }

        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<T>()
                .AddAsync(entity, cancellationToken);
        }
        public async Task<IReadOnlyList<T>> ListAsync(
       ISpecification<T> specification,
       CancellationToken cancellationToken = default)
        {
            var query =
                ApplySpecification(specification);

            return await query.ToListAsync(
                cancellationToken);
        }
        public async Task<PaginatedResult<T>> ListPagedAsync(
         ISpecification<T> specification,
         int page,
         int pageSize,
         CancellationToken cancellationToken = default)
        {
            var query =
                ApplySpecification(specification).AsNoTracking();

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            var items =
                await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

            return new PaginatedResult<T>(
                items,
                totalCount,
                page,
                pageSize);
        }
    }
}
