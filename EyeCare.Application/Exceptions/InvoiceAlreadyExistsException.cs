namespace EyeCare.Application.Exceptions;

public sealed class InvoiceAlreadyExistsException : Exception
{
    public InvoiceAlreadyExistsException()
        : base("An invoice already exists for this appointment.")
    {
    }
}