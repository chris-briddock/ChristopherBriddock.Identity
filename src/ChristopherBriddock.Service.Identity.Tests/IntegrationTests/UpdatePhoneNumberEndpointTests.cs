using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class UpdatePhoneNumberEndpointTests
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

    private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();
        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
         userManagerMock.Setup(x => x.GenerateChangePhoneNumberTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync("token");

        return userManagerMock;

    }

    private ClaimsPrincipal CreateClaimsPrincipalMock(string email)
    {
        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(u => u.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, email));
        return claimsPrincipalMock.Object;
    }

    private HttpClient CreateClientWithMocks(UserManager<ApplicationUser> userManagerMock, IHttpContextAccessor httpContextAccessorMock)
    {
        return _fixture.CreateAuthenticatedClient(services =>
        {
            services.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock));
            services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock));
        });
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
    public async Task UpdatePhoneNumberEndpoint_Returns204NoContent_WhenUpdateIsSuccessful() 
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePhoneNumberRequest request = new UpdatePhoneNumberRequest
        {
            PhoneNumber = "000000000000"
        };

        var response = await client.PutAsJsonAsync("/account/phonenumber", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task UpdatePhoneNumberEndpoint_Returns400BadRequest_WhenUpdateFails() 
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePhoneNumberRequest request = new UpdatePhoneNumberRequest
        {
            PhoneNumber = "000000000000"
        };

        var response = await client.PutAsJsonAsync("/account/phonenumber", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdatePhoneNumberEndpoint_Returns500InternalServerError_WhenExceptionIsThrown() 
    {
        var claimsPrincipalMock = CreateClaimsPrincipalMock("test@test.com");
        var httpContextAccessorMock = CreateHttpContextAccessorMock(claimsPrincipalMock);
        var userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(x => x.ChangePhoneNumberAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());
        var client = CreateClientWithMocks(userManagerMock.Object, httpContextAccessorMock);

        UpdatePhoneNumberRequest request = new UpdatePhoneNumberRequest
        {
            PhoneNumber = "000000000000"
        };

        var response = await client.PutAsJsonAsync("/account/phonenumber", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}