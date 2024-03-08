namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents the request parameters for registering an external application.
/// </summary>
public sealed record  RegisterApplicationRequest 
{
    /// <summary>
    /// Gets or sets the name of the external application.
    /// </summary>
    public required string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the website of the external application.
    /// </summary>
    public required string Website { get; set; } = default!;
    /// <summary>
    /// Gets or sets the redirect url of the external application.
    /// </summary>
    public required string RedirectUri { get; set; } = default!;
}