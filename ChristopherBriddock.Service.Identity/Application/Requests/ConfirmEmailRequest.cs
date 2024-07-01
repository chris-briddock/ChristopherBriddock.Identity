using Microsoft.AspNetCore.Mvc;

namespace Application.Requests;

/// <summary>
/// Represents an email confirmation.
/// </summary>
public sealed record ConfirmEmailRequest
{
    /// <summary>
    /// The users email address
    /// </summary>
    [FromQuery(Name = "email_address")]
    public required string EmailAddress { get; init; } = default!;
    /// <summary>
    /// The code to confirm the email address
    /// </summary>
    [FromQuery(Name = "code")]
    public required string Code { get; init; } = default!;

}
