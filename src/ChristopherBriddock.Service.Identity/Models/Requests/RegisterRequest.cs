namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a user registering for the application.
/// </summary>
public sealed record RegisterRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public required string EmailAddress { get; init; } = default!;
    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; } = default!;
    /// <summary>
    /// The user's phone number.
    /// </summary>
    public required string PhoneNumber { get; init; } = default!;
}
