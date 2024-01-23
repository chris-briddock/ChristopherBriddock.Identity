using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class EmailPublisherTests
{
    [Fact]
    public async Task PublishIsSuccessfulWithCorrectMessage()
    {
        var bus = new IBusMock().Mock();

        var messageContents = new EmailMessage()
        {
            EmailAddress = "Testing",
            Type = EmailPublisherConstants.Register,
            Code = "abcdef"
        };

        await bus.Publish(messageContents);

        await bus.Received(1).Publish(messageContents);
    }
}
