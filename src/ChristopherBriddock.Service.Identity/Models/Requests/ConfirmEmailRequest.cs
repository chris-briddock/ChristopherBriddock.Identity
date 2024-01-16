namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents an email confirmation.
/// </summary>
public sealed record ConfirmEmailRequest
{
    /// <summary>
    /// The users email address
    /// </summary>
    public required string EmailAddress { get; set; } = default!;
    /// <summary>
    /// The code to confirm the email address
    /// </summary>
    public required string Code { get; set; } = default!;


}
