using System.IdentityModel.Tokens.Jwt;

public class JwtSecurityTokenHandlerMock : Mock<JwtSecurityTokenHandler>, IMockBase<JwtSecurityTokenHandlerMock>
{
    public JwtSecurityTokenHandlerMock Mock()
    {
        return this;
    }
}