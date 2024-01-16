using ChristopherBriddock.Service.Identity.Models;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Sends a message to the message queue, for confirmation emails, 
/// password reset codes, password reset links and two factor codes.
/// </summary>
public class EmailProvider : IEmailProvider
{
    /// <inheritdoc/>
    public Task SendConfirmationLinkAsync(ApplicationUser user,
                                          string email,
                                          string confirmationLink)
    {
        // TODO: implement this
        // send message to message queue.
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public Task SendPasswordResetCodeAsync(ApplicationUser user,
                                           string email,
                                           string resetCode)
    {
        // TODO: implement this
        // send message to message queue.
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public Task SendPasswordResetLinkAsync(ApplicationUser user,
                                           string email,
                                           string resetLink)
    {
        // TODO: implement this
        // send message to message queue.
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public Task SendTwoFactorCodeAsync(ApplicationUser user,
                                       string email,
                                       string twoFactorCode)
    {
        // TODO: implement this
        // send message to message queue.
        return Task.CompletedTask;
    }
}
