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
/// Exposes an endpoint to allow the user to update their password.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="UpdatePasswordEndpoint"/>
/// </remarks>
/// <param name="services"></param>
/// <param name="logger"></param>
public class UpdatePasswordEndpoint(IServiceProvider services,
                                    ILogger<UpdatePasswordEndpoint> logger) : EndpointBaseAsync
                                                                             .WithRequest<UpdatePasswordRequest>
                                                                             .WithoutParam
                                                                             .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;
    /// <summary>
    /// The application's logger.
    /// </summary>
    public ILogger<UpdatePasswordEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to update their password.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/account/password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<ActionResult> HandleAsync(UpdatePasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetService<UserManager<ApplicationUser>>()!;
            string emailAddress = User.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(emailAddress);

            if (user is null)
            {
                return NotFound();
            }
            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(UpdatePasswordEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
