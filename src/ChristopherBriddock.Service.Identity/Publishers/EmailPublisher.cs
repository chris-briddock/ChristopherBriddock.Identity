using ChristopherBriddock.Service.Common.Messaging;
using MassTransit;

namespace ChristopherBriddock.Service.Identity.Publishers;

/// <summary>
/// Publishes a message to the message queue, for confirmation emails, 
/// password reset codes, password reset links and two factor codes.
/// </summary>
/// <remarks>
/// Initalizes a new instance of <see cref="EmailPublisher"/>
/// </remarks>
public sealed class EmailPublisher(IBus bus) : IEmailPublisher
{
    /// <summary>
    /// A bus is a logical element that includes a local endpoint and zero or more receive endpoints
    /// </summary>
    public IBus Bus { get; } = bus;

    /// <inheritdoc/>
    public async Task Publish(EmailMessage emailMessage,
                              CancellationToken cancellationToken)
    {
        await Bus.Publish(emailMessage, cancellationToken);
    }

}
