using Microsoft.AspNetCore.Mvc;

namespace Application.Requests;

/// <summary>
/// Represents a request for authorization.
/// </summary>
public sealed record AuthorizeRequest
{
    /// <summary>
    /// Gets or sets the email address associated with the user.
    /// </summary>
    /// <remarks>
    /// This field is required for user identification.
    /// </remarks>
    [FromBody]
    public string? EmailAddress { get; init; } = default!;

    /// <summary>
    /// Gets or sets the password associated with the user.
    /// </summary>
    /// <remarks>
    /// This field is required for user authentication.
    /// </remarks>
    [FromBody]
    public string? Password { get; init; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether the user's session should be remembered.
    /// </summary>
    /// <remarks>
    /// Setting this to <c>true</c> will remember the user's session across browser sessions.
    /// </remarks>
    [FromBody]
    public bool? RememberMe { get; init; } = default!;

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
    public string State { get; init; } = default!;
}
