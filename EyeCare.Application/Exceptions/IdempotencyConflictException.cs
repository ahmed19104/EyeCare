namespace EyeCare.Application.Exceptions;

public sealed class IdempotencyConflictException : Exception
{
    public IdempotencyConflictException()
        : base("An idempotency conflict occurred.")
    {
    }
    
}