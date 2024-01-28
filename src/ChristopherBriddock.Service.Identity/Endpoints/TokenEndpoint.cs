using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models.Responses;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint to allow the user to generate a token.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="TokenEndpoint"/>
/// </remarks>
/// <param name="services">The application's service provider.</param>
/// <param name="logger">The application logger.</param>
public class TokenEndpoint(IServiceProvider services,
                           ILogger<TokenEndpoint> logger) : EndpointBaseAsync
                                                            .WithoutRequest
                                                            .WithActionResult<TokenResponse>
{
    /// <summary>
    /// The service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    public ILogger<TokenEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to generate a Bearer token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpGet("/token")]
    [Authorize(Policy = "UserRolePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<TokenResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var httpContextAccessor = Services.GetService<IHttpContextAccessor>()!;
            var configuration = Services.GetService<IConfiguration>()!;
            var jsonWebTokenProvider = Services.GetService<IJsonWebTokenProvider>()!;

            string email = httpContextAccessor.HttpContext!.User.Identity!.Name ?? string.Empty;
            string issuer = configuration["Jwt:Issuer"]!;
            string audience = configuration["Jwt:Audience"]!;
            string secret = configuration["Jwt:Secret"]!;
            string subject = httpContextAccessor.HttpContext!.User.Identity!.Name ?? string.Empty;
            string expires = configuration["Jwt:Expires"]!;

            var result = await jsonWebTokenProvider.TryCreateTokenAsync(email,
                                                                        secret,
                                                                        issuer,
                                                                        audience,
                                                                        expires,
                                                                        subject);
            var refreshToken = await jsonWebTokenProvider.TryCreateRefreshTokenAsync(email,
                                                                                     secret,
                                                                                     issuer,
                                                                                     audience,
                                                                                     expires,
                                                                                     subject);
            if (result.Success && refreshToken.Success)
            {
                bool isAccessTokenValid = (await jsonWebTokenProvider.TryValidateTokenAsync(result.Token,
                                                                           secret,
                                                                           issuer,
                                                                           audience)).Success;
                bool isRefreshTokenValid = (await jsonWebTokenProvider.TryValidateTokenAsync(refreshToken.Token,
                                                                                             secret,
                                                                                             issuer,
                                                                                             audience)).Success;
                if (!isAccessTokenValid && !isRefreshTokenValid)
                {
                    return Unauthorized();
                }


            }
            TokenResponse response = new()
            {
                AccessToken = result.Token,
                RefreshToken = refreshToken.Token,
                Expires = expires
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(TokenEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
