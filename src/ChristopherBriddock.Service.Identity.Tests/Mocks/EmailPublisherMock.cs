using ChristopherBriddock.Service.Identity.Publishers;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class EmailPublisherMock : Mock<IEmailPublisher>, IMockBase<EmailPublisherMock>
{
    public EmailPublisherMock Mock()
    {
        return this;
    }
}
