namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a password update request.
/// </summary>
public sealed record UpdatePasswordRequest
{
    /// <summary>
    /// The user's current password.
    /// </summary>
    public required string CurrentPassword { get; init; } = default!;
    /// <summary>
    /// The user's new password.
    /// </summary>
    public required string NewPassword { get; init; } = default!;
}
