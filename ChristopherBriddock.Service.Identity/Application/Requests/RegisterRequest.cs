using Domain.ValueObjects;

namespace Application.Requests;

/// <summary>
/// Represents a user registering for the application.
/// </summary>
public sealed record RegisterRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public required string Email { get; init; } = default!;
    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; } = default!;
    /// <summary>
    /// The user's phone number.
    /// </summary>
    public required string PhoneNumber { get; init; } = default!;
    /// <summary>
    /// The user's address details.
    /// </summary>
    public required Address Address { get; init; } = default!;
}
