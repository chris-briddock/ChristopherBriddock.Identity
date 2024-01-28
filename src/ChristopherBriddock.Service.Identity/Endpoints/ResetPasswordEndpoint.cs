using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint to allow a user to reset their password.
/// </summary>
/// <param name="services">The application service provider.</param>
/// <param name="logger">The logger.</param>
public sealed class ResetPasswordEndpoint(IServiceProvider services,
                                          ILogger<ResetPasswordEndpoint> logger) : EndpointBaseAsync
                                                                                  .WithRequest<ResetPasswordRequest>
                                                                                  .WithoutParam
                                                                                  .WithActionResult
{
    /// <summary>
    /// The application service provier.
    /// </summary>
    private IServiceProvider Services { get; } = services;
    /// <summary>
    /// The applications logger.
    /// </summary>
    private ILogger<ResetPasswordEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to reset their password.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/resetpassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] ResetPasswordRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetService<UserManager<ApplicationUser>>()!;

            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email);

            bool isConfirmed = await userManager.IsEmailConfirmedAsync(user!);

            if (isConfirmed)
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(request.ResetCode);
                var code = Encoding.UTF8.GetString(decodedBytes);
                await userManager.ResetPasswordAsync(user!, code, request.NewPassword);
                return NoContent();
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(ResetPasswordEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
