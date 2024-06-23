namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

public class JsonWebTokenProviderMock : Mock<IJsonWebTokenProvider>, IMockBase<JsonWebTokenProviderMock>
{
    public JsonWebTokenProviderMock Mock()
    {
        return this;
    }
}
