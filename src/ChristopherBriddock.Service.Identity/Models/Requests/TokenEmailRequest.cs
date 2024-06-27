namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a token email request.
/// </summary>
public sealed record TokenEmailRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public string EmailAddress { get; init; } = default!;

    /// <summary>
    /// The type of token to be generated.
    /// </summary>
    public string TokenType { get; init; } = default!;
}
