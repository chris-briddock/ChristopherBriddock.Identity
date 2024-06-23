using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models.Entities;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// An endpoint which allows confirming the users email address.
/// </summary>
/// <remarks>
/// Initializses a new instance of <see cref="ConfirmEmailEndpoint"/>
/// </remarks>
/// <param name="serviceProvider"> The <see cref="IServiceProvider"/> </param>
/// <param name="logger">The logger for this endpoint.</param>
public sealed class ConfirmEmailEndpoint(IServiceProvider serviceProvider,
                                         ILogger<ConfirmEmailEndpoint> logger) : EndpointBaseAsync
                                                                                 .WithRequest<ConfirmEmailRequest>
                                                                                 .WithoutQuery
                                                                                 .WithActionResult
{
    /// <inheritdoc/>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    private ILogger<ConfirmEmailEndpoint> Logger { get; } = logger;
    /// <summary>
    /// Allows a user to confirm their email address.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/confirmemail")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(ConfirmEmailRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(request.EmailAddress);

            // NOTE: This code should have been emailed to the user.

            IdentityResult result = await userManager.ConfirmEmailAsync(user!, request.Code);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors.First());
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(ConfirmEmailEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
