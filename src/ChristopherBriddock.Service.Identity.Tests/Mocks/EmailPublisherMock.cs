using ChristopherBriddock.Service.Identity.Publishers;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks
{
    internal class EmailPublisherMock : Mock<IEmailPublisher>, IMockBase<EmailPublisherMock>
    {
        public EmailPublisherMock Mock()
        {
            return this;
        }
    }
}
