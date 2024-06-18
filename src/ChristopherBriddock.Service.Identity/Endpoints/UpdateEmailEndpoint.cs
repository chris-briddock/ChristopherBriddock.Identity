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
/// Exposes an endpoint to allow the user to update their email.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="UpdateEmailEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">The application logger.</param>
public class UpdateEmailEndpoint(IServiceProvider serviceProvider,
                                 ILogger<UpdateEmailEndpoint> logger) : EndpointBaseAsync
                                                                        .WithRequest<UpdateEmailRequest>
                                                                        .WithoutQuery
                                                                        .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; } = serviceProvider;
    /// <summary>
    /// The application's logger.
    /// </summary>
    public ILogger<UpdateEmailEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to update their email address.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPut("/account/email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdateEmailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var httpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            var userClaimsPrincipal = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!;
            var user = await userManager.FindByEmailAsync(userClaimsPrincipal.Value);

            user!.EmailConfirmed = false;
            await userManager.UpdateAsync(user);
            await userManager.SetUserNameAsync(user, request.EmailAddress);
            var token = await userManager.GenerateChangeEmailTokenAsync(user, request.EmailAddress);
            var result = await userManager.ChangeEmailAsync(user, request.EmailAddress, token);

            if (!result.Succeeded)
            {
                return BadRequest();
            }
            // At this point send an email to re-confirm the user's email.
            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(UpdateEmailEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
