using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Models.Responses;
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
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The application's logger.</param>
public class RegisterExternalApplicationEndpoint(IServiceProvider serviceProvider,
                             ILogger<RegisterExternalApplicationEndpoint> logger) : EndpointBaseAsync
                                                                                    .WithRequest<RegisterApplicationRequest>
                                                                                    .WithoutQuery
                                                                                    .WithActionResult
{
    /// <summary>
    /// The application service provider.
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    private ILogger<RegisterExternalApplicationEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to register a new application.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/register/app")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<ActionResult> HandleAsync([FromBody] RegisterApplicationRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = ServiceProvider.GetRequiredService<AppDbContext>();
            var cryptoProvider = ServiceProvider.GetRequiredService<ICryptoProvider>();
            var codeProvider = ServiceProvider.GetRequiredService<ICodeProvider>();
            string secret = codeProvider.Create(256);

            Application application = new() 
            {
                Name = request.Name,
                Website = request.Website,
                RedirectUri = request.RedirectUri,
                ClientId = Guid.NewGuid(),
                ClientSecret = cryptoProvider.GenerateHash(secret)
            };
            
            await context.AspNetApplications.AddAsync(application, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            RegisterApplicationResponse response = new() 
            {
                ClientId = application.ClientId,
                ClientSecret = secret
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(RefreshEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}