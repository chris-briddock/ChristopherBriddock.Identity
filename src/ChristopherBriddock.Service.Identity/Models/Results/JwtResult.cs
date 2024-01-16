namespace ChristopherBriddock.Service.Identity.Models.Results;

/// <summary>
/// Represents the result of a JWT (JSON Web Token) operation.
/// </summary>
public sealed class JwtResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the JWT operation was successful.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Gets or sets the JWT (JSON Web Token) string if the operation was successful.
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Gets or sets an optional error message in case the JWT operation encountered an error.
    /// </summary>
    public string? Error { get; set; } = default!;
}
