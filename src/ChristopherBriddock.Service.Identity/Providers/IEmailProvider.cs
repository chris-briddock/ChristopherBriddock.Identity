using ChristopherBriddock.Service.Identity.Models;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// An extension to <see cref="Microsoft.AspNetCore.Identity.IEmailSender{TUser}"/>
/// </summary>
/// <remarks>
/// Exposes an asyncronous task that allows the user to send a two factor code via email.
/// </remarks>
public interface IEmailProvider : Microsoft.AspNetCore.Identity.IEmailSender<ApplicationUser>
{
    /// <summary>
    /// This API supports the ASP.NET Core Identity infrastructure and is not intended to be used as a general purpose
    /// email abstraction. It should be implemented by the application so the Identity infrastructure can send password reset emails.
    /// </summary>
    /// <param name="user">The user that is recieiving the two factor code.</param>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="twoFactorCode">The code to use to login via two factor. Do not encode this.</param>
    Task SendTwoFactorCodeAsync(ApplicationUser user, string email, string twoFactorCode);
}
