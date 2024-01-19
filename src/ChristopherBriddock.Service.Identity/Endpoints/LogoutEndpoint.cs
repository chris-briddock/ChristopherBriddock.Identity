﻿using Ardalis.ApiEndpoints;
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
/// <param name="logger">The application's logger.</param>
public sealed class LogoutEndpoint(IServiceProvider services,
                                    ILogger<LogoutEndpoint> logger) : EndpointBaseAsync
                                                                      .WithoutRequest
                                                                      .WithActionResult
{

    /// <inheritdoc/>
    public IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    public ILogger<LogoutEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to sign out.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var signInManeger = Services.GetRequiredService<SignInManager<ApplicationUser>>();

            await signInManeger.SignOutAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in endpoint: {nameof(LogoutEndpoint)} - {nameof(HandleAsync)} Error details: {ex}", ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
