using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
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
/// Initializes a new instance of <see cref="DeleteAccountEndpoint"/>
/// </remarks>
public class DeleteAccountEndpoint(ILogger<DeleteAccountEndpoint> logger,
                                   IServiceProvider serviceProvider) : EndpointBaseAsync
                                                                       .WithoutRequest
                                                                       .WithActionResult
{
    /// <summary>
    /// The application's service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; } = serviceProvider;
    /// <summary>
    /// The application's logger.
    /// </summary>
    public ILogger<DeleteAccountEndpoint> Logger { get; } = logger;

    /// <inheritdoc/>
    [HttpDelete("/account/delete")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string emailAddress = User.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(emailAddress);

            user!.IsDeleted = true;

            await userManager.UpdateAsync(user);

            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(DeleteAccountEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
