namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a token email request.
/// </summary>
public sealed record TwoFactorTokenEmailRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = default!;
}
