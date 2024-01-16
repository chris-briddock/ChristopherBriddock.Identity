namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a user email request.
/// </summary>
public sealed record UpdatePhoneNmberRequest
{
    /// <summary>
    /// The new email address.
    /// </summary>
    public string EmailAddress { get; set; }
}
