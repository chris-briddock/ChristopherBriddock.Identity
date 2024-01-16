namespace ChristopherBriddock.Service.Identity.Models.Requests;

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
    public required string EmailAddress { get; set; } = default!;

    /// <summary>
    /// Gets or sets the password associated with the user.
    /// </summary>
    /// <remarks>
    /// This field is required for user authentication.
    /// </remarks>
    public required string Password { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether the user's session should be remembered.
    /// </summary>
    /// <remarks>
    /// Setting this to <c>true</c> will remember the user's session across browser sessions.
    /// </remarks>
    public required bool RememberMe { get; set; } = default!;
}

