using ChristopherBriddock.Service.Common.Messaging;
using MassTransit;
using System.Text.Json;

namespace ChristopherBriddock.Service.Email;

/// <summary>
/// Initializes a new instance of <see cref="EmailConsumer"/>
/// </summary>
/// <param name="bus">A logical element that includes a local endpoint and zero or more receive endpoints</param>
public class EmailConsumer(ILogger<EmailConsumer> logger) : IConsumer<EmailMessage>
{

    public ILogger<EmailConsumer> Logger { get; } = logger;

    public async Task Consume(ConsumeContext<EmailMessage> context)
    {
        try
        {
            Logger.LogInformation("consuming message");
            var message = JsonSerializer.Serialize(context.Message);
            Logger.LogInformation(message);

            // TODO: Messaging works, inplement email sending.

        }
        catch(Exception ex)
        {
            throw;
        }
    }
}
