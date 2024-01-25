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
public sealed class EmailPublisher(IServiceProvider serviceProvider) : IEmailPublisher
{
    /// <summary>
    /// The application's service provider
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc/>
    public async Task Publish(EmailMessage emailMessage,
                              CancellationToken cancellationToken)
    {

        var bus = ServiceProvider.GetService<IBus>()!;
        var featureManager = ServiceProvider.GetService<IFeatureManager>()!;

        if (await featureManager!.IsEnabledAsync(FeatureFlagConstants.RabbitMq) ||
            await featureManager!.IsEnabledAsync(FeatureFlagConstants.AzServiceBus))
        {
            await bus.Publish(emailMessage, cancellationToken);
        }
    }

}
