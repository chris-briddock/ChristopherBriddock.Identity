namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a request to register a new OAuth 2.0 client application.
/// </summary>
public sealed record RegisterApplicationRequest 
{
    /// <summary>
    /// Gets or sets the name of the client application.
    /// This is a human-readable name for the application.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Gets or sets the callback URI for the client application.
    /// This URI is used by the authorization server to redirect the user after a successful authorization.
    /// </summary>
    public string CallbackUri { get; init; } = default!;
}
