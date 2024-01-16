using Ardalis.ApiEndpoints;
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
/// <param name="configuration">The application's configuration</param>
/// <param name="jsonWebTokenProvider">The json web token provider.</param>
/// <param name="services">The application's service provider.</param>
/// <param name="httpContextAccessor">Allows access to a <see cref="HttpContent"/> via an interface.</param>
public class TokenEndpoint(IConfiguration configuration,
                           IJsonWebTokenProvider jsonWebTokenProvider,
                           IServiceProvider services,
                           IHttpContextAccessor httpContextAccessor) : EndpointBaseAsync
                                                                      .WithoutRequest
                                                                      .WithActionResult<TokenResponse>
{
    /// <summary>
    /// The application's configuration.
    /// </summary>
    public IConfiguration Configuration { get; } = configuration;
    /// <summary>
    /// The json web token provider.
    /// </summary>
    public IJsonWebTokenProvider JsonWebTokenProvider { get; set; } = jsonWebTokenProvider;
    /// <summary>
    /// The service provider.
    /// </summary>
    public IServiceProvider Services { get; } = services;
    /// <summary>
    /// Allows access to a <see cref="HttpContent"/> via an interface.
    /// </summary>
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;

    /// <summary>
    /// Allows a user to generate a Bearer token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpGet("/token")]
    [Authorize(Policy = "UserRolePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public override async Task<ActionResult<TokenResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string email = HttpContextAccessor.HttpContext!.User.Identity!.Name ?? string.Empty;
            string issuer = Configuration["Jwt:Issuer"]!;
            string audience = Configuration["Jwt:Audience"]!;
            string secret = Configuration["Jwt:Secret"]!;
            string subject = HttpContextAccessor.HttpContext!.User.Identity!.Name ?? string.Empty;
            string expires = Configuration["Jwt:Expires"]!;

            var result = await JsonWebTokenProvider.TryCreateTokenAsync(email,
                                                                        secret,
                                                                        issuer,
                                                                        audience,
                                                                        expires,
                                                                        subject);
            var refreshToken = await JsonWebTokenProvider.TryCreateRefreshTokenAsync(email,
                                                                                     secret,
                                                                                     issuer,
                                                                                     audience,
                                                                                     expires,
                                                                                     subject);
            if (result.Success && refreshToken.Success)
            {
                bool isAccessTokenValid = (await JsonWebTokenProvider.TryValidateTokenAsync(result.Token,
                                                                           secret,
                                                                           issuer,
                                                                           audience)).Success;
                bool isRefreshTokenValid = (await JsonWebTokenProvider.TryValidateTokenAsync(refreshToken.Token,
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
            // TODO: Add Logging.
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
