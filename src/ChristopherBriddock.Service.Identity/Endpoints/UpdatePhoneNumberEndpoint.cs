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
                                                                                    .WithoutParam
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
    /// Allows a user to update their email address.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/account/phonenumber")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(UpdatePhoneNumberRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = ServiceProvider.GetService<UserManager<ApplicationUser>>()!;
            string emailAddress = User.FindFirst(ClaimTypes.Email)!.Value;

            var user = await userManager.FindByEmailAsync(emailAddress);

            if (user is null)
            {
                return NotFound();
            }

            var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);

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
