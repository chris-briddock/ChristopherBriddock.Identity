using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models.Entities;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for authorizing a user.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="AuthorizeEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger for this endpoint.</param>
[Route("api/v{version:apiVersion}/")]
public sealed class AuthorizeEndpoint(IServiceProvider serviceProvider,
                                      ILogger<AuthorizeEndpoint> logger) : EndpointBaseAsync
                                                                          .WithRequest<AuthorizeRequest>
                                                                          .WithQuery<AuthorizeQueryRequest>
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
    /// <param name="query">The object which encapsulates the query request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("oauth/authorize")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] AuthorizeRequest request,
                                                         [FromQuery] AuthorizeQueryRequest query,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var dbContext = ServiceProvider.GetRequiredService<AppDbContext>();

            var clientQuery = await dbContext
                                .Set<IdentityApplication>()
                                .Where(x => x.ClientId == query.ClientId)
                                .SingleOrDefaultAsync(cancellationToken);
            
            var storedRedirectUri = await dbContext
                                          .Set<IdentityApplication>()
                                          .Where(x => x.RedirectUri == query.RedirectUri)
                                          .SingleOrDefaultAsync(cancellationToken);
            
            if (clientQuery is null)
                return Unauthorized("Client is not registered.");
                
            if (query.RedirectUri != storedRedirectUri!.RedirectUri)
                return Unauthorized("Redirect uri does not match the value stored for this application");
            
            
            var signInManager = ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var signInResult = await signInManager.PasswordSignInAsync(request.EmailAddress,
                                                                       $"""{request.Password}""",
                                                                       request.RememberMe,
                                                                       lockoutOnFailure: true);

            if (signInResult.RequiresTwoFactor)
                return Ok("Two Factor is required");
                // Remember to call the /sendemail endpoint for the user.
                // then once email is confirmed, call the /2fa/authorize endpoint.

            if (!signInResult.Succeeded)
                return Unauthorized();

            ApplicationUser? user = await userManager.FindByEmailAsync(request.EmailAddress) ?? null!;

            if (user.IsDeleted)
                return Unauthorized();

            await signInManager.CreateUserPrincipalAsync(user);

            return Redirect(storedRedirectUri.RedirectUri);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(AuthorizeEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
