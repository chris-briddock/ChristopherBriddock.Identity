using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Publishers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint which sends a token via email.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="SendTokenEmailEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">The application's logger.</param>
public sealed class SendTokenEmailEndpoint(IServiceProvider serviceProvider,
                                                ILogger<SendTokenEmailEndpoint> logger) : EndpointBaseAsync
                                                                                          .WithRequest<TokenEmailRequest>
                                                                                          .WithoutQuery
                                                                                          .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public ILogger<SendTokenEmailEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Sends a token to the user by email.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("/sendemail")]
    public override async Task<ActionResult> HandleAsync([FromBody] TokenEmailRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            ApplicationUser? user;
            EmailMessage message = new();
            var userManager = ServiceProvider.GetService<UserManager<ApplicationUser>>()!;
            var emailPublisher = ServiceProvider.GetService<IEmailPublisher>()!;
            var linkProvider = ServiceProvider.GetService<ILinkProvider>()!;
            user = await userManager.FindByEmailAsync(request.EmailAddress);
            if (user is null)
                return NotFound();
            if (request.TokenType == EmailPublisherConstants.TwoFactorToken)
            {
                var code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultProvider);

                message = new()
                {
                    EmailAddress = user.Email!,
                    Code = code,
                    Type = EmailPublisherConstants.TwoFactorToken
                };
            }

            if (request.TokenType == EmailPublisherConstants.ConfirmEmail) 
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                RouteValueDictionary routeValues = new()
                {
                    ["EmailAddress"] = user.Email,
                    ["Code"] = code
                };

                Uri link = linkProvider.GetUri(HttpContext, "confirmemail", routeValues);

                message = new()
                {
                    EmailAddress = user.Email!,
                    Link = link.ToString(),
                    Type = EmailPublisherConstants.ConfirmEmail
                };

            }
            if (request.TokenType == EmailPublisherConstants.ForgotPassword)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);

                message = new()
                {
                    EmailAddress = user.Email!,
                    Code = code,
                    Type = EmailPublisherConstants.ForgotPassword
                };
            }   
            await emailPublisher.Publish(message, cancellationToken);
            return Ok("If a user exists in the database, an email will be sent to that email address.");
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(SendTokenEmailEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
