using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents the authorization query request.
/// </summary>
public sealed record AuthorizeQueryRequest 
{
    /// <summary>
    /// The client id of the application.
    /// </summary>
    [FromQuery(Name = "client_id")]
    public Guid ClientId { get; init; }
    /// <summary>
    /// The redirect uri the application will redirect to
    /// after a successful authentication attempt.
    /// </summary>
    [FromQuery(Name = "redirect_uri")]
    public string RedirectUri { get; init; } = default!;
    /// <summary>
    /// The token response type.
    /// </summary>
    /// <remarks>
    /// Options are: code, token 
    /// </remarks>
    [FromQuery(Name = "response_type")]
    public string ResponseType { get; init; } = default!;
    /// <summary>
    /// The scopes of the token. 
    /// </summary>
    [FromQuery(Name = "scopes")]
    public string? Scopes { get; init; } = default!;
    /// <summary>
    /// The state value automatically generated 
    /// when the application is registered.
    /// </summary>
    [FromQuery(Name = "state")]
    public string State { get; set; } = default!;
}