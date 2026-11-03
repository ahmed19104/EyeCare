using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Repositories
{
    public sealed class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetForFinancialUpdateAsync(
            Guid invoiceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .Include(x => x.Items)
                .Include(x => x.Payments)
                    .ThenInclude(x => x.Refunds).AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == invoiceId,
                    cancellationToken);
        }

    }
}
