using EyeCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.Abstractions.Persistence
{
    public interface IInvoiceRepository
    {
     
        Task<Invoice?> GetForFinancialUpdateAsync(
            Guid invoiceId,
            CancellationToken cancellationToken = default);
    }
}
