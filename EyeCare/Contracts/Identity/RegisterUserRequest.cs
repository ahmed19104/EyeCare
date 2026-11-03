namespace EyeCareAPI.Contracts.Identity
{
    public sealed record RegisterUserRequest(
     string FirstName,
     string LastName,
     string Email,
     string Password);
}
