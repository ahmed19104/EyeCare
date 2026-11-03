using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using System.Linq.Expressions;

namespace EyeCare.Application.Billing.Invoices.Specifications;

public sealed class InvoiceByIdSpecification
    : ISpecification<Invoice>
{
    public InvoiceByIdSpecification(Guid invoiceId)
    {
        Criteria = x => x.Id == invoiceId;
    }

    public Expression<Func<Invoice, bool>> Criteria { get; }

    public List<Expression<Func<Invoice, object>>> Includes =>
    [
        x => x.Items
    ];

    public Expression<Func<Invoice, object>>? OrderByDescending => null;
}