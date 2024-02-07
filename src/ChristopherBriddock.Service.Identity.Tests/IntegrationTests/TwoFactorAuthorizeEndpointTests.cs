using ChristopherBriddock.Service.Identity.Endpoints;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorAuthorizeEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public TwoFactorAuthorizeEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
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

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(s => s.GetService(typeof(UserManager<ApplicationUser>)))
                           .Returns(userManagerMock.Object);

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IServiceProvider), serviceProviderMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsJsonAsync($"/2fa/authorize", request);

        Assert.Equivalent(HttpStatusCode.Found, response.StatusCode);
    }
}
