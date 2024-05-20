namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class TwoFactorAuthorizeEndpointTests 
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
    public async Task TwoFactorAuthorizeEndpoint_ReturnsStatus302Found_WhenValidTokenIsUsed()
    {
        TwoFactorSignInRequest request = new()
        {
            EmailAddress = "abdfe@gmail.com",
            Token = Convert.ToBase64String(new byte[256])
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        var user = new ApplicationUser();

        userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        userManagerMock.Setup(s => s.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        userManagerMock.Setup(m => m.VerifyTwoFactorTokenAsync(It.IsAny<ApplicationUser>(),
                                                               It.IsAny<string>(),
                                                               It.IsAny<string>())).ReturnsAsync(true);

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var sut = await client.PostAsJsonAsync($"/2fa/authorize", request);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Found));
    }
}
