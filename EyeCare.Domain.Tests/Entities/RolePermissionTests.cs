
using EyeCare.Domain.Entities;
using Xunit;

namespace EyeCare.Domain.Tests.Entities;

public class RolePermissionTests
{
    [Fact]
    public void Constructor_ShouldCreateRolePermission_WithValidData()
    {
        // Arrange
        var roleId = "admin-role";
        var permission = "Patients.Read";

        // Act
        var rolePermission = new RolePermission(
            roleId,
            permission);

        // Assert
        Assert.Equal(roleId, rolePermission.RoleId);
        Assert.Equal(permission, rolePermission.Permission);
    }

    [Fact]
    public void Constructor_ShouldPreserveRoleIdExactly()
    {
        var roleId = "  admin-role  ";

        var rolePermission = new RolePermission(
            roleId,
            "Patients.Read");

        Assert.Equal(roleId, rolePermission.RoleId);
    }

    [Fact]
    public void Constructor_ShouldPreservePermissionExactly()
    {
        var permission = "  Patients.Read  ";

        var rolePermission = new RolePermission(
            "admin-role",
            permission);

        Assert.Equal(permission, rolePermission.Permission);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsEmptyRoleId()
    {
        // يوثّق السلوك الحالي؛ الـ constructor لا يتحقق من RoleId.
        var rolePermission = new RolePermission(
            "",
            "Patients.Read");

        Assert.Equal("", rolePermission.RoleId);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsEmptyPermission()
    {
        // يوثّق السلوك الحالي؛ الـ constructor لا يتحقق من Permission.
        var rolePermission = new RolePermission(
            "admin-role",
            "");

        Assert.Equal("", rolePermission.Permission);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsNullRoleId()
    {
        // الـ parameter معلن string، لكن ممكن تمرير null وقت التشغيل.
        var rolePermission = new RolePermission(
            null!,
            "Patients.Read");

        Assert.Null(rolePermission.RoleId);
    }

    [Fact]
    public void Constructor_CurrentlyAllowsNullPermission()
    {
        var rolePermission = new RolePermission(
            "admin-role",
            null!);

        Assert.Null(rolePermission.Permission);
    }

    [Fact]
    public void Constructor_ShouldCreateDifferentObjects_ForDifferentPermissions()
    {
        var readPermission = new RolePermission(
            "admin-role",
            "Patients.Read");

        var writePermission = new RolePermission(
            "admin-role",
            "Patients.Write");

        Assert.Equal("admin-role", readPermission.RoleId);
        Assert.Equal("admin-role", writePermission.RoleId);

        Assert.NotEqual(
            readPermission.Permission,
            writePermission.Permission);
    }

    [Fact]
    public void Constructor_ShouldAllowSameRoleAndPermission_OnSeparateInstances()
    {
        // التحقق من منع التكرار مسؤول عنه غالبًا الـ database constraint
        // أو الـ service التي تضيف الصلاحيات، وليس هذا الـ constructor.
        var first = new RolePermission(
            "admin-role",
            "Patients.Read");

        var second = new RolePermission(
            "admin-role",
            "Patients.Read");

        Assert.Equal(first.RoleId, second.RoleId);
        Assert.Equal(first.Permission, second.Permission);
        Assert.NotSame(first, second);
    }
}
