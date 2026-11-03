namespace EyeCare.Application.Abstractions.Security;

public interface ICurrentUser
{
    string? UserId { get; }
}
