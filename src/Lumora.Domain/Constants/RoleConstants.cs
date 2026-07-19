namespace Lumora.Domain.Constants;

public static class RoleConstants
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin      = "Admin";
    public const string Student    = "Student";

    public static readonly string[] All = [SuperAdmin, Admin, Student];

    // Roles allowed into the Admin area
    public static readonly string[] AdminRoles = [SuperAdmin, Admin];
}
