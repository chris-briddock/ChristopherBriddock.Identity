using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for redeeming two factor recovery codes.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TwoFactorRecoveryCodesRedeemEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">Rhe application's logger.</param>
public class TwoFactorRecoveryCodesRedeemEndpoint(IServiceProvider serviceProvider,
                                                  ILogger<TwoFactorRecoveryCodesEndpoint> logger) : EndpointBaseAsync
                                                                                                    .WithRequest<TwoFactorRecoveryCodesRedeemRequest>
                                                                                                    .WithoutQuery
                                                                                                    .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public ILogger<TwoFactorRecoveryCodesEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to redeem two factor recovery codes.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/2fa/redeem")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] TwoFactorRecoveryCodesRedeemRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(request.EmailAddress);

            var result = await userManager.RedeemTwoFactorRecoveryCodeAsync(user!, request.Code);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TwoFactorRecoveryCodesRedeemEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
