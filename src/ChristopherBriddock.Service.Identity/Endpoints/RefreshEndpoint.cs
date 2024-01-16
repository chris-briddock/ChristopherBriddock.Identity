using Ardalis.ApiEndpoints;
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
/// <param name="jsonWebTokenProvider"></param>
/// <param name="configuration"></param>
public class RefreshEndpoint(IJsonWebTokenProvider jsonWebTokenProvider,
                             IConfiguration configuration) : EndpointBaseAsync
                                                             .WithRequest<RefreshRequest>
                                                             .WithActionResult
{
    /// <inheritdoc/>
    public IJsonWebTokenProvider JsonWebTokenProvider { get; } = jsonWebTokenProvider;
    /// <inheritdoc/>
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Allows a user to refresh the bearer token.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpGet("/refresh")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<ActionResult> HandleAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await JsonWebTokenProvider.TryValidateTokenAsync(request.RefreshToken,
                                                   Configuration["Jwt:Secret"]!,
                                                   Configuration["Jwt:Issuer"]!,
                                                   Configuration["Jwt:Audience"]!);
        if (!validationResult.Success)
        {
            return Unauthorized();
        }
        return await Task.FromResult(LocalRedirect("/token"));
    }
}
