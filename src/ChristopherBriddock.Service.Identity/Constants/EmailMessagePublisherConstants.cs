namespace ChristopherBriddock.Service.Identity.Constants;

/// <summary>
/// Represents the types of email messages that can be sent from the application to the message queue.
/// </summary>
public static class EmailMessagePublisherConstants
{
    /// <summary>
    /// The value for a forogtten password message.
    /// </summary>
    public const string ForgotPassword = "ForgotPassword";
    /// <summary>
    /// The value for a two factor token message.
    /// </summary>
    public const string TwoFactorToken = "TwoFactorToken";
    /// <summary>
    /// The value for a register message
    /// </summary>
    public const string Register = "Register";
}
