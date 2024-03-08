namespace ChristopherBriddock.Service.Identity.Models;

/// <summary>
/// Represents an application.
/// </summary>
public sealed record Application
{
    /// <summary>
    /// Gets or sets the identifer for the application.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Gets or sets the application website.
    /// </summary>
    public string Website { get; set; } = default!;
    /// <summary>
    /// Gets or sets the redirect uri for the registered application.
    /// </summary>
    public string RedirectUri { get; set; } = default!;
    /// <summary>
    /// Gets or sets the client id of the application.
    /// </summary>
    public Guid ClientId { get; set; } = default!;
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    public string ClientSecret { get; set; } = default!;
}