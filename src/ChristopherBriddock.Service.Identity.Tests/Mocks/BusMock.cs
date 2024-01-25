using MassTransit;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class BusMock : Mock<IBus>, IMockBase<BusMock>
{
    public BusMock Mock()
    {
        return this;
    }
}
