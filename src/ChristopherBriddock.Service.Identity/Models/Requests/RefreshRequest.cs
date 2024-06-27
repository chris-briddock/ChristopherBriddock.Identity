namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a refresh request.
/// </summary>
public sealed record RefreshRequest
{
    /// <summary>
    /// The refresh token.
    /// </summary>
    public required string RefreshToken { get; init; } = default!;
}
