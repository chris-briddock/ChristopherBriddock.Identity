using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models.Entities;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint for registering a new application.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="RegisterApplicationEndpoint"/>
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger for this endpoint.</param>
[Route("api/v{version:apiVersion}/")]
public sealed class RegisterApplicationEndpoint(IServiceProvider serviceProvider,
                                                ILogger<AuthorizeEndpoint> logger) : EndpointBaseAsync
                                                                                    .WithRequest<RegisterApplicationRequest>
                                                                                    .WithoutQuery
                                                                                    .WithActionResult
{
    /// <summary>
    /// The service provider.
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <summary>
    /// The logger for this endpoint.
    /// </summary>
    private ILogger<AuthorizeEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to add a new application.
    /// </summary>
    /// <param name="request">The object which encapsulates the request body.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("applications")]
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(RegisterApplicationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var dbContext = ServiceProvider.GetRequiredService<AppDbContext>();

            IdentityApplication application = new()
            {
                Name = request.Name,
                RedirectUri = request.CallbackUri
            };
            
            await dbContext.AspNetApplications.AddAsync(application, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);



            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(RegisterApplicationEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
