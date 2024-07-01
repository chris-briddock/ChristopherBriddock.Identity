using Application.Commands;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

/// <summary>
/// Exposes an endpoint that allows the user to soft delete their account.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="DeleteAccountEndpoint"/>
/// </remarks>
[Route("api/v{version:apiVersion}/")]
public class DeleteAccountEndpoint : EndpointBaseAsync
                                     .WithoutRequest
                                     .WithActionResult
{
    /// <summary>
    /// The application's service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }
    /// <summary>
    /// The application's logger.
    /// </summary>
    public ILogger<DeleteAccountEndpoint> Logger { get; }
    public DeleteAccountEndpoint(IServiceProvider serviceProvider,
                                ILogger<DeleteAccountEndpoint> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteAccountCommand = ServiceProvider.GetRequiredService<DeleteAccountCommand>();
            var result = await deleteAccountCommand.ExecuteAsync();

            return Ok();
        }
        catch (Exception ex)
        {

        }
    }
}
