namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a forgot password request.
/// </summary>
public sealed record ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public required string EmailAddress { get; set; } = default!;
}
