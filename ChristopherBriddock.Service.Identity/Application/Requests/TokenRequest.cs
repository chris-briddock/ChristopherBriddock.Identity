using Microsoft.AspNetCore.Mvc;

namespace Application.Requests;

/// <summary>
/// Represents a token request, for a specific token type.
/// </summary>
public sealed record TokenRequest
{
    /// <summary>
    /// The token type requested.
    /// </summary>
    [FromForm(Name = "grant_type")]
    public string GrantType { get; init; } = default!;
    /// <summary>
    /// Optional refresh token.
    /// </summary>
    [FromForm(Name = "refresh_token")]
    public string? RefreshToken { get; init; } = default!;
    /// <summary>
    /// Code for the grant type authorization_code
    /// </summary>
    [FromForm(Name = "code")]
    public string? Code { get; set; } = default!;
    /// <summary>
    /// The application client id.
    /// </summary>
    [FromForm(Name = "client_id")]
    public Guid ClientId { get; set; } = default!;
    /// <summary>
    /// The application secret.
    /// </summary>
    [FromForm(Name = "client_secret")]
    public string ClientSecret { get; set; } = default!;
}