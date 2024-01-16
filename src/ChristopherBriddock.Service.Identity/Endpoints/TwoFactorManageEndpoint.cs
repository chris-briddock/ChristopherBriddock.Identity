using Ardalis.ApiEndpoints;
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
/// <param name="services">The application's service provider.</param>
public sealed class TwoFactorManageEndpoint(IServiceProvider services) : EndpointBaseAsync
                                                                                       .WithRequest<TwoFactorManageRequest>
                                                                                       .WithActionResult
{
    /// <summary>
    /// The service provider for the application.
    /// </summary>
    public IServiceProvider Services { get; set; } = services;

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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(TwoFactorManageRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

            string emailAddress = User.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(emailAddress);

            if (user == null)
                return NotFound("User is not found.");

            if (await userManager.GetTwoFactorEnabledAsync(user))
                return BadRequest("User has two factor enabled.");

            var result = await userManager.SetTwoFactorEnabledAsync(user, request.IsEnabled);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return NoContent();

        }
        catch (Exception ex)
        {
            // TODO: Add Logging.
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
