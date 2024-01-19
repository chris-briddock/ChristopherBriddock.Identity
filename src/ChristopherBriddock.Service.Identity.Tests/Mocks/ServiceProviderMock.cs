using NSubstitute;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class ServiceProviderMock : MockBase<IServiceProvider>
{
    public override IServiceProvider Mock()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();

        return serviceProvider;
    }
}
