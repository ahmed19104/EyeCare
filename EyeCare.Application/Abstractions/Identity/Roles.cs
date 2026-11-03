namespace EyeCare.Application.Abstractions.Identity;

public static class Roles
{
    public const string Admin = "Admin";
    public const string BranchManager = "BranchManager";
    public const string Receptionist = "Receptionist";
    public const string Doctor = "Doctor";
    public const string Accountant = "Accountant";

    public static readonly IReadOnlyList<string> All =
    [
        Admin,
        BranchManager,
        Receptionist,
        Doctor,
        Accountant
    ];
}
