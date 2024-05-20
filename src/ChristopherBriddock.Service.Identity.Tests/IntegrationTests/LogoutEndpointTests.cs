using System.Net.Http.Headers;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class LogoutEndpointTests
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
    public async Task LogoutEndpoint_Returns401Unauthorized_WhenNotLoggedIn()
    {
        using var client = _fixture.WebApplicationFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        using var sut = await client.PostAsync("/logout", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task LogoutEndpoint_Returns204NoContent_WhenUserLogsOutSuccessfully()
    {
        var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsync("/logout", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task LogoutEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        var signInManagerMock = new SignInManagerMock<ApplicationUser>().Mock();

        signInManagerMock.Setup(s => s.SignOutAsync()).ThrowsAsync(new Exception());

        using var sutClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(SignInManager<ApplicationUser>), signInManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await sutClient.PostAsync("/logout", null);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}