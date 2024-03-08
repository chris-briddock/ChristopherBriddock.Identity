namespace ChristopherBriddock.Service.Identity.Models.Responses;

/// <summary>
/// Represents the response after registering an application.
/// </summary>
public sealed record RegisterApplicationResponse 
{
    /// <summary>
    /// Gets or sets the unique identifier of the registered client.
    /// </summary>
    public Guid ClientId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the client secret generated for the registered client.
    /// </summary>
    public string ClientSecret { get; set; } = default!;
}
