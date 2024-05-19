using MassTransit;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class BusMock : Mock<IBus>, IMockBase<BusMock>
{
    public BusMock Mock()
    {
        return this;
    }
}
