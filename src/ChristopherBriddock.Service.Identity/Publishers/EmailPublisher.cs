using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using MassTransit;
using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Identity.Publishers;

/// <summary>
/// Publishes a message to the message queue, for confirmation emails, 
/// password reset codes, password reset links and two factor codes.
/// </summary>
/// <remarks>
/// Initalizes a new instance of <see cref="EmailPublisher"/>
/// </remarks>
public sealed class EmailPublisher(IBus bus, IFeatureManager featureManager) : IEmailPublisher
{
    /// <summary>
    /// A bus is a logical element that includes a local endpoint and zero or more receive endpoints
    /// </summary>
    public IBus Bus { get; } = bus;
    /// <summary>
    /// The application's feature manager.
    /// </summary>
    public IFeatureManager FeatureManager { get; } = featureManager;

    /// <inheritdoc/>
    public async Task Publish(EmailMessage emailMessage,
                              CancellationToken cancellationToken)
    {

        if (await FeatureManager.IsEnabledAsync(FeatureFlagConstants.RabbitMq) ||
            await FeatureManager.IsEnabledAsync(FeatureFlagConstants.AzServiceBus))
        {
            await Bus.Publish(emailMessage, cancellationToken);
        }
    }

}
