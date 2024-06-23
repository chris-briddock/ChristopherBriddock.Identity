using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models.Entities;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for authorizing a user.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="AuthorizeEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger for this endpoint.</param>
public sealed class AuthorizeEndpoint(IServiceProvider serviceProvider,
                                      ILogger<AuthorizeEndpoint> logger) : EndpointBaseAsync
                                                                          .WithRequest<AuthorizeRequest>
                                                                          .WithoutQuery
                                                                          .WithActionResult
{
    /// <summary>
    /// The service provider.
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <summary>
    /// The logger for this endpoint.
    /// </summary>
    private ILogger<AuthorizeEndpoint> Logger { get; } = logger;


    /// <summary>
    /// Allows a user to be authorized.
    /// </summary>
    /// <param name="request">The object which encapsulates the request body.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/authorize")]
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
            var signInManager = ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var signInResult = await signInManager.PasswordSignInAsync(request.EmailAddress,
                                                                       $"""{request.Password}""",
                                                                       request.RememberMe,
                                                                       lockoutOnFailure: true);

            if (signInResult.RequiresTwoFactor)
            {
                return Ok("Two Factor is required");
                // Remember to call the /sendemail endpoint for the user.
                // then once email is confirmed, call the /2fa/authorize endpoint.
            }

            if (!signInResult.Succeeded)
            {
                return Unauthorized();
            }

            ApplicationUser? user = await userManager.FindByEmailAsync(request.EmailAddress);

            if (user!.IsDeleted)
            {
                return Unauthorized();
            }

            await signInManager.CreateUserPrincipalAsync(user!);

            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(AuthorizeEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
