using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for authorizing a user.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="AuthoriseEndpoint"/>
/// </remarks>
/// <param name="services">The service provider.</param>
/// <param name="logger">The logger for this endpoint.</param>
public sealed class AuthoriseEndpoint(IServiceProvider services,
                                      ILogger<AuthoriseEndpoint> logger) : EndpointBaseAsync
                                                                          .WithRequest<AuthorizeRequest>
                                                                          .WithoutParam
                                                                          .WithActionResult
{
    /// <summary>
    /// The service provider.
    /// </summary>
    private IServiceProvider Services { get; } = services;
    /// <summary>
    /// The logger for this endpoint.
    /// </summary>
    private ILogger<AuthoriseEndpoint> Logger { get; } = logger;


    /// <summary>
    /// Allows a user to be authorized.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/authorise")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] AuthorizeRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var signInManager = Services.GetService<SignInManager<ApplicationUser>>()!;

            var userManager = Services.GetService<UserManager<ApplicationUser>>()!;

            signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var signInResult = await signInManager.PasswordSignInAsync(request.EmailAddress,
                                                                       request.Password,
                                                                       request.RememberMe,
                                                                       lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                return Unauthorized();
            }

            if (signInResult.RequiresTwoFactor)
            {
                return LocalRedirect("/2fa/email");
            }

            ApplicationUser? user = await userManager.FindByEmailAsync(request.EmailAddress);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await signInManager.CreateUserPrincipalAsync(user);

            return LocalRedirect($"/token");
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(AuthoriseEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
