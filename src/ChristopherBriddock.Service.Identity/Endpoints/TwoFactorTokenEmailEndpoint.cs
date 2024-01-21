using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Publishers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint which sends a 2fa token via email.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TwoFactorTokenEmailEndpoint"/>
/// </remarks>
/// <param name="services">The application service provider.</param>
/// <param name="emailPublisher">Allows the sending of emails for verification purposes.</param>
/// <param name="logger">The application's logger.</param>
public sealed class TwoFactorTokenEmailEndpoint(IServiceProvider services,
                                                IEmailPublisher emailPublisher,
                                                ILogger<TwoFactorTokenEmailEndpoint> logger) : EndpointBaseAsync
                                                                                               .WithRequest<TwoFactorTokenEmailRequest>
                                                                                               .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    public IEmailPublisher EmailPublisher { get; } = emailPublisher;
    /// <inheritdoc/>
    public ILogger<TwoFactorTokenEmailEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Sends a two factor token to the user by email.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("/2fa/email")]
    public override async Task<ActionResult> HandleAsync(TwoFactorTokenEmailRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            ApplicationUser? user;
            var userEmail = request.Email;
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
            if (userEmail is null)
                return BadRequest();

            user = await userManager.FindByEmailAsync(userEmail);

            if (user is null)
                return NotFound();

            var code = await userManager.GenerateTwoFactorTokenAsync(user!, TokenOptions.DefaultProvider);

            EmailMessage message = new()
            {
                EmailAddress = user.Email!,
                Code = code,
                Type = EmailPublisherConstants.TwoFactorToken
            };
            await EmailPublisher.Publish(message, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorTokenEmailEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


    }
}
