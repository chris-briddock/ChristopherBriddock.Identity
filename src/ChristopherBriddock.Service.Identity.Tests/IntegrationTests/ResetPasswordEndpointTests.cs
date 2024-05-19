using Microsoft.AspNetCore.Identity.Data;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class ResetPasswordEndpointTests
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
    public async Task ResetEndpoint_ReturnsStatus204_WhenResetIsSuccessful()
    {
        ResetPasswordRequest resetPasswordRequest = new()
        {
            Email = "test@test.com",
            ResetCode = Convert.ToBase64String(new byte[256]),
            NewPassword = "abcdjenwejfnskdnc"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);
        userManagerMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()));


        using var resetClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await resetClient.PostAsJsonAsync("/resetpassword", resetPasswordRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task ResetEndpoint_ReturnsStatus500_WhenExceptionIsThrown()
    {
        ResetPasswordRequest resetPasswordRequest = new()
        {
            Email = "test@test.com",
            ResetCode = "abcdeefjdk",
            NewPassword = "abcdjenwejfnskdnc"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ApplicationUser>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>())).ThrowsAsync(new Exception());


        using var resetClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        using var sut = await resetClient.PostAsJsonAsync("/resetpassword", resetPasswordRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}
