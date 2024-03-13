using ChristopherBriddock.Service.Common.Constants;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class SendTokenEmailEndpointTests : IClassFixture<WebApplicationFactoryCustom>
{
    private readonly WebApplicationFactoryCustom _webApplicationFactory;
    private TokenEmailRequest _sendTokenEmailRequest;

    public SendTokenEmailEndpointTests(WebApplicationFactoryCustom webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _sendTokenEmailRequest = default!;
    }

    public static IEnumerable<object[]> GetTokenType()
    {
        yield return new object[] { EmailPublisherConstants.TwoFactorToken };
        yield return new object[] { EmailPublisherConstants.ConfirmEmail };
        yield return new object[] { EmailPublisherConstants.ForgotPassword };
    }

    [Theory]
    [MemberData(nameof(GetTokenType))]
    public async Task SendTokenEmailEndpoint_ReturnsStatus200OK_WhenRequestIsValid(string tokenType)
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        _sendTokenEmailRequest = new TokenEmailRequest
        {
            EmailAddress = "test@test.com",
            TokenType = tokenType
        };

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();
        

        var response = await client.PostAsJsonAsync("/sendemail", _sendTokenEmailRequest);
        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SendTokenEmailEndpoint_ReturnsStatus400BadRequest_WhenNoEmailAddressPassed()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();
        _sendTokenEmailRequest = new TokenEmailRequest 
        {
            EmailAddress = null!,
            TokenType = EmailPublisherConstants.ConfirmEmail 
        };
 
        var response = await client.PostAsJsonAsync("/sendemail", _sendTokenEmailRequest);
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendTokenEmailEndpoint_ReturnsStatus404NotFound_WhenEmailAddressIsInvalid()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();
        _sendTokenEmailRequest = new TokenEmailRequest { EmailAddress = "test@invalid.com", TokenType = EmailPublisherConstants.TwoFactorToken };

        var response = await client.PostAsJsonAsync("/sendemail", _sendTokenEmailRequest);
        Assert.Equivalent(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task SendTokenEmailEndpoint_ReturnsStatus500InternalServerError_WhenAnExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception());
        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        _sendTokenEmailRequest = new TokenEmailRequest { EmailAddress = "test@invalid.com", TokenType = EmailPublisherConstants.TwoFactorToken };

        var response = await client.PostAsJsonAsync("/sendemail", _sendTokenEmailRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}