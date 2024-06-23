using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models.Entities;
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
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">The logger.</param>
public class UpdatePasswordEndpoint(IServiceProvider serviceProvider,
                                    ILogger<UpdatePasswordEndpoint> logger) : EndpointBaseAsync
                                                                             .WithRequest<UpdatePasswordRequest>
                                                                             .WithoutQuery
                                                                             .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
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
    [HttpPut("/account/password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdatePasswordRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var httpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            var userClaimsPrincipal = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!;

            var user = await userManager.FindByEmailAsync(userClaimsPrincipal.Value);

            user!.ModifiedBy = user.Id;
            
            user!.ModifiedOnUtc = DateTime.UtcNow;

            var result = await userManager.ChangePasswordAsync(user!,
                                                               request.CurrentPassword,
                                                               request.NewPassword);

            if (!result.Succeeded)
                return BadRequest();

            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(UpdatePasswordEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
