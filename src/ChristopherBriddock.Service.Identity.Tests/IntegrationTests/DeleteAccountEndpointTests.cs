using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class DeleteAccountEndpointTests
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
    }

    [Test]
    public async Task DeleteAccountEndpoint_Returns204NoContent_WhenAccountIsDeleted()
    {

        // Create user manager mock
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        // Setup user manager mock behaviour
        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        // Create a mock instance of ClaimsPrincipal and set up behavior for FindFirst method
        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(u => u.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, "test@test.com"));

        // Create a mock instance of HttpContext and set up the User property
        var httpContextMock = new HttpContextMock();
        httpContextMock.Setup(x => x.User).Returns(claimsPrincipalMock.Object);

        // Create a mock instance of IHttpContextAccessor
        var httpContextAccessorMock = new IHttpContextAccessorMock();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        using var sutClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
            });
        }).CreateClient();

        using var sut = await sutClient.DeleteAsync("/account/delete");

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteAccountEndpoint_Returns500_WhenExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.DeleteAsync(It.IsAny<ApplicationUser>())).ThrowsAsync(new Exception());

        using var clientWithForcedError = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await clientWithForcedError.DeleteAsync("/account/delete");

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task DeleteAccountEndpoint_Returns401_WhenUserIsUnauthorized()
    {
        using var client = _fixture.WebApplicationFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        using var sut = await client.DeleteAsync("/account/delete");

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteAccountEndpoint_Returns500_WhenUserIsNotFound()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(value: null);

        using var clientWithForcedError = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await clientWithForcedError.DeleteAsync("/account/delete");

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

}
