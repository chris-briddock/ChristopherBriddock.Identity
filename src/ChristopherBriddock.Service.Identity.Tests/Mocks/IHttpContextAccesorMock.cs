using Microsoft.AspNetCore.Http;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class IHttpContextAccessorMock : Mock<IHttpContextAccessor>, IMockBase<IHttpContextAccessorMock>
{
    public IHttpContextAccessorMock Mock()
    {
        return this;
    }
}