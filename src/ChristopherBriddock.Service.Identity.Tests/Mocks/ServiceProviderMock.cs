namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class ServiceProviderMock : Mock<IServiceProvider>, IMockBase<ServiceProviderMock>
{
    public ServiceProviderMock Mock()
    {
        return this;
    }
}
