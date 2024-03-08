using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represent the query parameters for an authorize request. 
/// </summary>
public sealed record AuthorizeRequestQuery 
{
    /// <summary>
    /// Gets or sets the specified response type.
    /// </summary>
    [FromQuery(Name = "response_type")]
    public string? ResponseType { get; set; } = default!;
    /// <summary>
    /// Gets or sets the client id.
    /// </summary>
    [FromQuery(Name = "client_id")]
    public Guid ClientId { get; set; } = default!;
    /// <summary>
    /// Gets or sets the redirect uri.
    /// </summary>
    [FromQuery(Name = "redirect_uri")]
    public string? RedirectUri { get; set; } = default!;
    /// <summary>
    /// Gets or sets the scopes within the JWT.
    /// </summary>
    [FromQuery(Name = "scopes")]
    public string? Scopes { get; set; } = default!;
}