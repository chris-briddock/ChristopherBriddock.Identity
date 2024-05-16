using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint that allows the user to enable two factor.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TwoFactorManageEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application's service provider.</param>
/// <param name="logger">The application's logger.</param>
public sealed class TwoFactorManageEndpoint(IServiceProvider serviceProvider,
                                            ILogger<TwoFactorManageEndpoint> logger) : EndpointBaseAsync
                                                                                       .WithRequest<TwoFactorManageRequest>
                                                                                       .WithoutQuery
                                                                                       .WithActionResult
{
    /// <summary>
    /// The service provider for the application.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public ILogger<TwoFactorManageEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to enable two factor.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/2fa/manage")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromQuery] TwoFactorManageRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetService<UserManager<ApplicationUser>>()!;

            IHttpContextAccessor httpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            Claim? emailClaim = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!;

            var user = await userManager.FindByEmailAsync(emailClaim.Value);

            var result = await userManager.SetTwoFactorEnabledAsync(user!, request.IsEnabled);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to enable two factor.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorManageEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
