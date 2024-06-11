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
/// Exposes an endpoint to allow the user to update their phone number.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="UpdatePhoneNumberEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The application service provider.</param>
/// <param name="logger">The application logger.</param>
public class UpdatePhoneNumberEndpoint(IServiceProvider serviceProvider,
                                       ILogger<UpdatePhoneNumberEndpoint> logger) : EndpointBaseAsync
                                                                                    .WithRequest<UpdatePhoneNumberRequest>
                                                                                    .WithoutQuery
                                                                                    .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <summary>
    /// The application logger.
    /// </summary>
    public ILogger<UpdatePhoneNumberEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to update their phone number.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPut("/account/phonenumber")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdatePhoneNumberRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var httpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            var userClaimsPrincipal = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!;
            var user = await userManager.FindByEmailAsync(userClaimsPrincipal.Value);

            var token = await userManager.GenerateChangePhoneNumberTokenAsync(user!, user!.PhoneNumber!);

            var result = await userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, token);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(UpdatePhoneNumberEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
