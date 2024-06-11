using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class UpdatePasswordEndpointTests
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

    private HttpClient CreateClientWithMocks(UserManager<ApplicationUser> userManagerMock, IHttpContextAccessor httpContextAccessorMock)
    {
        return _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock));
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions());
    }

    private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        return userManagerMock;

    }

    private ClaimsPrincipal CreateClaimsPrincipalMock(string email)
    {
        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(u => u.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, email));
        return claimsPrincipalMock.Object;
    }

    private IHttpContextAccessor CreateHttpContextAccessorMock(ClaimsPrincipal claimsPrincipalMock)
    {
        var httpContextAccessorMock = new IHttpContextAccessorMock();
        var httpContextMock = new HttpContextMock();

        httpContextMock.Setup(x => x.User).Returns(claimsPrincipalMock);
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        return httpContextAccessorMock.Object;
    }

    [Test]
    public async Task UpdatePasswordEndpoint_Returns204NoContent_WhenUpdateIsSuccessful()
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePasswordRequest request = new UpdatePasswordRequest
        {
            CurrentPassword = "XXXX",
            NewPassword = "XXXXX"
        };

        var response = await client.PutAsJsonAsync("/account/password", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task UpdatePasswordEndpoint_Returns400BadRequest_WhenUpdateIsFails()
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePasswordRequest request = new UpdatePasswordRequest
        {
            CurrentPassword = "XXXX",
            NewPassword = "XXXXX"
        };

        var response = await client.PutAsJsonAsync("/account/password", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdatePasswordEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePasswordRequest request = new UpdatePasswordRequest
        {
            CurrentPassword = "XXXX",
            NewPassword = "XXXXX"
        };

        var response = await client.PutAsJsonAsync("/account/password", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}