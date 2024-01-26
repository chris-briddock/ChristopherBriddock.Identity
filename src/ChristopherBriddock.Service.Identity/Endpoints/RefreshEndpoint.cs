﻿using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint so the user can refresh the bearer token.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="RefreshEndpoint"/>
/// </remarks>
/// <param name="services">The service provider.</param>
/// <param name="logger">The application's logger.</param>
public class RefreshEndpoint(IServiceProvider services,
                             ILogger<RefreshEndpoint> logger) : EndpointBaseAsync
                                                               .WithRequest<RefreshRequest>
                                                               .WithoutParam
                                                               .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    private IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    private ILogger<RefreshEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to refresh the bearer token.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpGet("/refresh")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<ActionResult> HandleAsync([FromBody] RefreshRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        var jsonWebTokenProvider = Services.GetService<IJsonWebTokenProvider>()!;
        var configuration = Services.GetService<IConfiguration>()!;

        var validationResult = await jsonWebTokenProvider.TryValidateTokenAsync(request.RefreshToken,
                                                                                configuration["Jwt:Secret"]!,
                                                                                configuration["Jwt:Issuer"]!,
                                                                                configuration["Jwt:Audience"]!);
        if (!validationResult.Success)
        {
            return Unauthorized();
        }
        return LocalRedirect("/token");
    }
}
