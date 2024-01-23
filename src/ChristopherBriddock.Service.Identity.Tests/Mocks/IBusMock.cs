using MassTransit;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class IBusMock : MockBase<IBus>
{
    public override IBus Mock()
    {
        return Substitute.For<IBus>();
    }
}
