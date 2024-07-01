using Application.Requests;
using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Application.Commands.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public class ConfirmEmailEndpoint : EndpointBaseAsync
                                    .WithRequest<ConfirmEmailRequest>
                                    .WithActionResult
{

    public IServiceProvider ServiceProvider { get; }

    public ILogger<ConfirmEmailEndpoint> Logger { get; }

    public ConfirmEmailEndpoint(IServiceProvider serviceProvider,
                                ILogger<ConfirmEmailEndpoint> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }


    public override async Task<ActionResult> HandleAsync(ConfirmEmailRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        try
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();
            
            await mediator.Send(request, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(ConfirmEmailEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
