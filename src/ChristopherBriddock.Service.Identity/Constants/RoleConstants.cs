namespace ChristopherBriddock.Service.Identity.Constants;

/// <summary>
/// Constant values for roles available in the system by default.
/// </summary>
/// <remarks>
/// These roles will be embedded in the user's JWT token, as a role claim.
/// </remarks>
public static class RoleConstants
{
    /// <summary>
    /// Represents the user role.
    /// </summary>
    /// <remarks>
    /// The minimum amount of access.
    /// </remarks>
    public const string UserRole = "User";

    /// <summary>
    /// Represents the admin role.
    /// </summary>
    /// <remarks>
    /// The maximum amount of access.
    /// </remarks>
    public const string AdminRole = "Admin";
}
