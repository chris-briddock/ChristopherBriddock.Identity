namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a user's phone number, which is to be updated.
/// </summary>
public sealed record UpdatePhoneNumberRequest
{
    /// <summary>
    /// The new phone number.
    /// </summary>
    public string PhoneNumber { get; set; } = default!;
}
