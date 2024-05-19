namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class ServiceProviderMock : Mock<IServiceProvider>, IMockBase<ServiceProviderMock>
{
    public ServiceProviderMock Mock()
    {
        return this;
    }
}
