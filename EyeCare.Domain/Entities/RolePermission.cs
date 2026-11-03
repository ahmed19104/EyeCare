namespace EyeCare.Domain.Entities;

public class RolePermission
{
    private RolePermission()
    {
    }

    public RolePermission(
        string roleId,
        string permission)
    {
        RoleId = roleId;
        Permission = permission;
    }

    public string RoleId { get; private set; } = null!;

    public string Permission { get; private set; } = null!;
}