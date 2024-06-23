namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class ConfirmEmailEndpointTests
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

    [Test]
    public async Task ConfirmEmailEndpoint_ReturnsStatus500_WithInvalidRequest()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        var confirmEmailRequest = new ConfirmEmailRequest
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        using var sut = await client.PostAsync($"/confirmemail?email_address=test%40test.com&code={confirmEmailRequest.Code}", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task ConfirmEmailEndpoint_ReturnsStatus200OK_WithValidRequest()
    {
        var confirmEmailRequest = new ConfirmEmailRequest
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        using var sut = await client.PostAsync($"/confirmemail?email_address=test%40test.com&code={confirmEmailRequest.Code}", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task ConfirmEmailEndpoint_ReturnsStatus500_WhenUserIsNotFound()
    {
        var confirmEmailRequest = new ConfirmEmailRequest
        {
            EmailAddress = "test@asdf.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            // No UserManager setup here to simulate user not found scenario.
        }).CreateClient();

        using var sut = await client.PostAsync($"/confirmemail?email_address={confirmEmailRequest.EmailAddress}&code={confirmEmailRequest.Code}", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}