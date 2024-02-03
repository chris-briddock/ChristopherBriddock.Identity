using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint which generates two factor recovery codes.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TwoFactorRecoveryCodesEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">The application's logger.</param>
public class TwoFactorRecoveryCodesEndpoint(IServiceProvider serviceProvider,
                                            ILogger<TwoFactorRecoveryCodesEndpoint> logger) : EndpointBaseAsync
                                                                                              .WithoutRequest
                                                                                              .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public ILogger<TwoFactorRecoveryCodesEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to generate two factor recovery codes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpGet("/2fa/codes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetService<UserManager<ApplicationUser>>()!;
            var email = User.FindFirst(ClaimTypes.Email)!.Value;
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return NotFound();
            }
            var codes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return Ok(codes);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorRecoveryCodesEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
