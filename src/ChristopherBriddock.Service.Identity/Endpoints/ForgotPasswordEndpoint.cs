using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Publishers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for resetting the password of a user.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="ForgotPasswordEndpoint"/>
/// </remarks>
/// <param name="services">The applications service provider.</param>
/// <param name="emailSender">The email sender.</param>
/// <param name="logger">The application logger</param>
public sealed class ForgotPasswordEndpoint(IServiceProvider services,
                                           IEmailPublisher emailSender,
                                           ILogger<ForgotPasswordEndpoint> logger) : EndpointBaseAsync
                                                                                    .WithRequest<ForgotPasswordRequest>
                                                                                    .WithActionResult
{
    /// <inheritdoc/>
    private IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    private IEmailPublisher EmailPublisher { get; } = emailSender;
    /// <inheritdoc/>
    public ILogger<ForgotPasswordEndpoint> Logger { get; set; } = logger;

    /// <summary>
    /// Allows a user to send a password reset email.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/forgotpassword")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] ForgotPasswordRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(request.EmailAddress);
            
            if (user is not null)
            {
                if (await userManager.IsEmailConfirmedAsync(user))
                {
                    return Conflict("Email is not confirmed.");
                }
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                EmailMessage message = new()
                {
                    EmailAddress = user.Email!,
                    Code = code,
                    Type = EmailPublisherConstants.ForgotPassword
                };
                await EmailPublisher.Publish(message, cancellationToken);

                return NoContent();
            }

            return NotFound();
            
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(ForgotPasswordEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
