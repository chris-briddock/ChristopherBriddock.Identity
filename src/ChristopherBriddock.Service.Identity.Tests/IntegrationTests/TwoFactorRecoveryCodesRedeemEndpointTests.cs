using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorRecoveryCodesRedeemEndpointTests : IClassFixture<WebApplicationFactory<Program>> 
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public TwoFactorRecoveryCodesRedeemEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns200OK_WhenOperationIsSuccessful()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

         var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
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

        var sut = await sutClient.PostAsJsonAsync("/2fa/redeem", redeemRequest);

        Assert.Equivalent(HttpStatusCode.OK, sut.StatusCode);
    }

    [Fact]
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns400BadRequest_WhenOperationIsNotSuccessful()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

         using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
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

        Assert.Equivalent(HttpStatusCode.BadRequest, sut.StatusCode);
    }

    [Fact]
    public async Task TwoFactorRecoveryCodesRedeemEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

        userManagerMock.Setup(s => s.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

         using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
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

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }
}