using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint that allows the user to sign in using two factor authentication.
/// </summary>
public sealed class TwoFactorAuthorizeEndpoint(IServiceProvider serviceProvider,
                                               ILogger<TwoFactorAuthorizeEndpoint> logger) : EndpointBaseAsync
                                                                                             .WithRequest<TwoFactorSignInRequest>
                                                                                             .WithoutQuery
                                                                                             .WithActionResult
{
    /// <summary>
    /// The service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public ILogger<TwoFactorAuthorizeEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows the user to sign in with two factor code.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/2fa/authorize")]
    [Authorize(AuthenticationSchemes = "Identity.Application", Policy = "UserRolePolicy")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] TwoFactorSignInRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(request.EmailAddress);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            bool isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);

            if (!isTwoFactorEnabled)
            {
                return Unauthorized("User does not have two factor enabled.");
            }
            var result = await userManager.VerifyTwoFactorTokenAsync(user,
                                                                     TokenOptions.DefaultProvider,
                                                                     request.Token);
            if (!result)
            {
                return Unauthorized("Two factor token could not be verified.");
            }
            return LocalRedirect("/token");
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorAuthorizeEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
