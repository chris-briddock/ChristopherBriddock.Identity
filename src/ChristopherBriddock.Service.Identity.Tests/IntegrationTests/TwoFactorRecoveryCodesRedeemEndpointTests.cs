namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorRecoveryCodesRedeemEndpointTests
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
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns200OK_WhenOperationIsSuccessful()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

         var sutClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(x =>
            {
                x.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TwoFactorRecoveryCodesRedeemRequest redeemRequest = new()
        {
            EmailAddress = "abde@gmail.com",
            Code = "sdkdmdkmksm"
        };

        using var sut = await sutClient.PostAsJsonAsync("/2fa/redeem", redeemRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns400BadRequest_WhenOperationIsNotSuccessful()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

         using var sutClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TwoFactorRecoveryCodesRedeemRequest redeemRequest = new()
        {
            EmailAddress = "abde@gmail.com",
            Code = "sdkdmdkmksm"
        };
        
        using var sut = await sutClient.PostAsJsonAsync("/2fa/redeem", redeemRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

         using var sutClient = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        TwoFactorRecoveryCodesRedeemRequest redeemRequest = new()
        {
            EmailAddress = "abde@gmail.com",
            Code = "sdkdmdkmksm"
        };
        
        using var sut = await sutClient.PostAsJsonAsync("/2fa/redeem", redeemRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}