namespace ChristopherBriddock.Service.Identity.Models.Responses;

/// <summary>
/// Represents the response when a user is authorized.
/// </summary>
public sealed record TokenResponse
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public required string AccessToken { get; set; } = default!;
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public required string RefreshToken { get; set; } = default!;
    /// <summary>
    /// Gets or sets the expiration of the token.
    /// </summary>
    public required string Expires { get; set; } = default!;
}
