using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a token request, for a specific token type.
/// </summary>
public sealed record TokenRequest
{
    /// <summary>
    /// The token type requested.
    /// </summary>
    [FromQuery(Name = "token_type")]
    public string TokenType { get; set; } = default!;
}