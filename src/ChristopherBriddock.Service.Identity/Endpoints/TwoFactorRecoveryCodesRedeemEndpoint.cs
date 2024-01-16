using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for redeeming two factor recovery codes.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TwoFactorRecoveryCodesRedeemEndpoint"/>
/// </remarks>
/// <param name="services">The application service provider.</param>
public class TwoFactorRecoveryCodesRedeemEndpoint(IServiceProvider services) : EndpointBaseAsync
                                                                               .WithRequest<TwoFactorRecoveryCodesRedeemRequest>
                                                                               .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(TwoFactorRecoveryCodesRedeemRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

            var email = User.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound();
            }

            var result = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, request.Code);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
        catch (Exception ex)
        {
            // TODO: Add Logging.
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


    }
}
