using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Invoices.Specifications;

public sealed class InvoiceDetailsSpecification
    : ISpecification<Invoice>
{
    public InvoiceDetailsSpecification(Guid invoiceId)
    {
        Criteria = x => x.Id == invoiceId;
    }

    public Expression<Func<Invoice, bool>> Criteria { get; }

    public List<Expression<Func<Invoice, object>>> Includes =>
    [
        x => x.Items,
        x => x.Payments
    ];

    public Expression<Func<Invoice, object>>? OrderByDescending => null;
}