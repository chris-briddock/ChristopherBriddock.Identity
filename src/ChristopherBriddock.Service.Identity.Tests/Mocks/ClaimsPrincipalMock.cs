using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class ClaimsPrincipalMock : Mock<ClaimsPrincipal>, IMockBase<ClaimsPrincipalMock>
{
    public ClaimsPrincipalMock Mock()
    {
        return this;
    }
}