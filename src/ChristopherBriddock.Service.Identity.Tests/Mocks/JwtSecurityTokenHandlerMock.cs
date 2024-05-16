using System.IdentityModel.Tokens.Jwt;

internal class JwtSecurityTokenHandlerMock : Mock<JwtSecurityTokenHandler>, IMockBase<JwtSecurityTokenHandlerMock>
{
    public JwtSecurityTokenHandlerMock Mock()
    {
        return this;
    }
}