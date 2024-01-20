using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint that allows the user to sign in using two factor authentication.
/// </summary>
public sealed class TwoFactorAuthoriseEndpoint(IServiceProvider services,
                                               IHttpContextAccessor httpContext,
                                               ILogger<TwoFactorAuthoriseEndpoint> logger) : EndpointBaseAsync
                                                                                              .WithRequest<TwoFactorSignInRequest>
                                                                                              .WithActionResult
{
    /// <summary>
    /// The service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;
    /// <summary>
    /// The application's http context accessor.
    /// </summary>
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContext;
    /// <inheritdoc/>
    public ILogger<TwoFactorAuthoriseEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows the user to sign in with two factor code.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/2fa/authorise")]
    [AllowAnonymous]
    public override async Task<ActionResult> HandleAsync(TwoFactorSignInRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        try
        {
            var signInManager = Services.GetRequiredService<SignInManager<ApplicationUser>>();
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(HttpContextAccessor.HttpContext!.User!.Identity!.Name!);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);

            if (!isTwoFactorEnabled)
            {
                return Unauthorized("User does not have two factor enabled.");
            }

            var result = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, request.Token);

            if (!result)
            {
                return BadRequest();
            }

            return LocalRedirect("/token");
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorAuthoriseEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
