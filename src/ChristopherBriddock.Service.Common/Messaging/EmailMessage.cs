using ChristopherBriddock.Service.Common.Constants;

namespace ChristopherBriddock.Service.Common.Messaging;

/// <summary>
/// Represent an message to be published to the message queue for emails.
/// </summary>
public sealed class EmailMessage
{
    /// <summary>
    /// The email address to which the email should be sent to.
    /// </summary>
    public string EmailAddress { get; set; } = default!;
    /// <summary>
    /// The type of email to be sent to the message queue. <see cref="EmailPublisherConstants"/> 
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// The code for the user to enter sent via email.
    /// </summary>
    public string Code { get; set; } = default!;
}
