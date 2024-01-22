namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class ServiceProviderMock : MockBase<IServiceProvider>
{
    public override IServiceProvider Mock()
    {
        return Substitute.For<IServiceProvider>();
    }
}
