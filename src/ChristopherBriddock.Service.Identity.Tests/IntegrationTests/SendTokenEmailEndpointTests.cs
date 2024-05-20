using ChristopherBriddock.Service.Common.Constants;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class SendTokenEmailEndpointTests
{
    private TestFixture<Program> _fixture;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _fixture = new TestFixture<Program>();
        _fixture.OneTimeSetUp();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _fixture.OneTimeTearDown();
        _fixture.Dispose();
        _fixture = null!;
    }

    public static IEnumerable<object[]> GetTokenType()
    {
        yield return new object[] { EmailPublisherConstants.TwoFactorToken };
        yield return new object[] { EmailPublisherConstants.ConfirmEmail };
        yield return new object[] { EmailPublisherConstants.ForgotPassword };
    }

    [Test]
    [TestCaseSource(nameof(GetTokenType))]
    public async Task SendTokenEmailEndpoint_ReturnsStatus200OK_WhenRequestIsValid(string tokenType)
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        TokenEmailRequest request = new()
        {
            EmailAddress = "test@test.com",
            TokenType = tokenType
        };

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

         using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        var sut = await client.PostAsJsonAsync("/sendemail", request);
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task SendTokenEmailEndpoint_ReturnsStatus400BadRequest_WhenNoEmailAddressPassed()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
         using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TokenEmailRequest request = new()
        {
            EmailAddress = null!,
            TokenType = EmailPublisherConstants.ConfirmEmail
        };

        var sut = await client.PostAsJsonAsync("/sendemail", request);
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task SendTokenEmailEndpoint_ReturnsStatus404NotFound_WhenEmailAddressIsInvalid()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        
        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TokenEmailRequest request = new() { EmailAddress = "test@invalid.com", TokenType = EmailPublisherConstants.TwoFactorToken };

        var sut = await client.PostAsJsonAsync("/sendemail", request);
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task SendTokenEmailEndpoint_ReturnsStatus500InternalServerError_WhenAnExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

         using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TokenEmailRequest request = new() { EmailAddress = "test@invalid.com", TokenType = EmailPublisherConstants.TwoFactorToken };

        var sut = await client.PostAsJsonAsync("/sendemail", request);
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}