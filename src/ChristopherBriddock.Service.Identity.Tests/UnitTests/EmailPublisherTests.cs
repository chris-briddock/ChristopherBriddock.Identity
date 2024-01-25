using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using ChristopherBriddock.Service.Identity.Publishers;
using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class EmailPublisherTests
{
    [Fact]
    public async Task PublishIsSuccessfulWithCorrectMessage()
    {
        var busMock = new BusMock();

        var featureManagerMock = new Mock<IFeatureManager>();

        featureManagerMock.Setup(s => s.IsEnabledAsync(FeatureFlagConstants.RabbitMq)).ReturnsAsync(true);

        EmailPublisher publisher = new(busMock.Object, featureManagerMock.Object);

        await publisher.Publish(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>());

        busMock.Verify(x => x.Publish(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()));
    }
}
