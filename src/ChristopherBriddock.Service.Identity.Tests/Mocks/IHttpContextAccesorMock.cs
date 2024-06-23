namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class IHttpContextAccessorMock : Mock<IHttpContextAccessor>, IMockBase<IHttpContextAccessorMock>
{
    public IHttpContextAccessorMock Mock()
    {
        return this;
    }
}