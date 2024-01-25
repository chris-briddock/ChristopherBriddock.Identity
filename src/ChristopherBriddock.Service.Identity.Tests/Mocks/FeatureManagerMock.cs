using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class FeatureManagerMock : Mock<IFeatureManager>, IMockBase<FeatureManagerMock>
{
    public FeatureManagerMock Mock()
    {
        return this;
    }
}
