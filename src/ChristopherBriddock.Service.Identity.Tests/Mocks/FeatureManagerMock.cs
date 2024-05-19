using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class FeatureManagerMock : Mock<IFeatureManager>, IMockBase<FeatureManagerMock>
{
    public FeatureManagerMock Mock()
    {
        return this;
    }
}
