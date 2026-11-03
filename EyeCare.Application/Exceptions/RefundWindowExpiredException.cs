namespace EyeCare.Application.Exceptions;

public sealed class RefundWindowExpiredException
    : Exception
{
    public RefundWindowExpiredException()
        : base("The refund window has expired.")
    {
    }
}
