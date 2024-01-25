using ChristopherBriddock.Service.Common.Messaging;

namespace ChristopherBriddock.Service.Identity.Publishers;

/// <summary>
/// This is a null implementation of <see cref="IEmailPublisher"/>
/// Allows for the system to be tested without needing a message queue available.
/// This will mean that the system will no longer be able to send emails for registration confirmation,
/// two factor authentication and forgot password, but this is only when the Feature Flag for RabbitMq or 
/// Azure Service Bus is disabled.
/// </summary>
public class NullEmailPublisher : IEmailPublisher
{
    public async Task Publish(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
