using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for logging out.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="LogoutEndpoint"/>
/// </remarks>
/// <param name="services">The application's service provider.</param>
public sealed class LogoutEndpoint(IServiceProvider services) : EndpointBaseAsync
                                                               .WithoutRequest
                                                               .WithActionResult
{

    /// <inheritdoc/>
    public IServiceProvider Services { get; set; } = services;
    /// <summary>
    /// Allows a user to sign out.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        var signInManeger = Services.GetRequiredService<SignInManager<ApplicationUser>>();

        await signInManeger.SignOutAsync();

        return NoContent();
    }
}
