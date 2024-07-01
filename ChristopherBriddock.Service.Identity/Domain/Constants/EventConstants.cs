namespace Domain.Constants;

/// <summary>
/// Contains constants representing various event types in the application.
/// </summary>
public static class EventConstants
{
    /// <summary>
    /// Event type for when an account is purged.
    /// </summary>
    public const string AccountPurged = "Account Purged";

    /// <summary>
    /// Event type for when an email address is confirmed.
    /// </summary>
    public const string EmailConfirmed = "Email Confirmed";

    /// <summary>
    /// Event type for when an email address is updated.
    /// </summary>
    public const string EmailUpdated = "Email Updated";

    /// <summary>
    /// Event type for when a login attempt is made.
    /// </summary>
    public const string LoginAttempted = "Login Attempted";

    /// <summary>
    /// Event type for when a password reset is requested.
    /// </summary>
    public const string PasswordResetRequested = "Password Reset Requested";

    /// <summary>
    /// Event type for when a password is updated.
    /// </summary>
    public const string PasswordUpdated = "Password Updated";

    /// <summary>
    /// Event type for when two-factor authentication is enabled.
    /// </summary>
    public const string TwoFactorEnabled = "Two Factor Enabled";

    /// <summary>
    /// Event type for when two-factor authentication is enabled.
    /// </summary>
    public const string TwoFactorDisabled = "Two Factor Disabled";

    /// <summary>
    /// Event type for when a user deletes their account.
    /// </summary>
    public const string UserDeletedAccount = "User Deleted Account";

    /// <summary>
    /// Event type for when a user registers a client application.
    /// </summary>
    public const string UserRegisteredClient = "User Registered Client Application";

    /// <summary>
    /// Event type for when a user removes a client application.
    /// </summary>
    public const string UserRemovedClient = "User Removed Client Application";

    /// <summary>
    /// Event type for when a user registers.
    /// </summary>
    public const string UserRegistered = "User Registered";
}

