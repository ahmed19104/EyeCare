namespace EyeCare.Application.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(
        string resourceName,
        Guid resourceId)
        : base($"{resourceName} with id '{resourceId}' was not found.")
        
    {
    }
    public NotFoundException(
     string resourceName,
     string resourceId)
     : base($"{resourceName} with id '{resourceId}' was not found.")

    {
    }
}
