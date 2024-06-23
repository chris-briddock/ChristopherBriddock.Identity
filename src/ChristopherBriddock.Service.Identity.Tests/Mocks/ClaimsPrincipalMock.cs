namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class ClaimsPrincipalMock : Mock<ClaimsPrincipal>, IMockBase<ClaimsPrincipalMock>
{
    public ClaimsPrincipalMock Mock()
    {
        return this;
    }
}