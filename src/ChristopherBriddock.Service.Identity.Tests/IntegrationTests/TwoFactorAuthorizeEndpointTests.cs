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

        var response = await client.GetAsync($"/2fa/authorize?EmailAddress=ssdjck@dmksksa.com&Token=cdcisdci");

        Assert.Equivalent(HttpStatusCode.Found, response.StatusCode);
    }

    [Fact]
    public async Task TwoFactorAuthorizeEndpoint_ReturnsStatus401Unauthorized_WhenInvalidTokenIsUsed()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();
        userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync(user);

        userManagerMock.Setup(m => m.VerifyTwoFactorTokenAsync(It.IsAny<ApplicationUser>(),
                                                               It.IsAny<string>(),
                                                               It.IsAny<string>())).ReturnsAsync(false);

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

        var response = await client.GetAsync($"/2fa/authorize?EmailAddress=test@test.com&Token=alslsslslsl");

        Assert.Equivalent(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TwoFactorAuthorizeEndpoint_ReturnsStatus500InternalServerError_WhenExceptionIsThrown()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Simulated exception"));

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(s => s.GetService(typeof(UserManager<ApplicationUser>)))
                           .Returns(userManagerMock.Object);

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IServiceProvider), serviceProviderMock.Object));
            });
        }).CreateClient();

        var response = await client.GetAsync("/2fa/authorize?EmailAddress=code@code.com&Token=ndcndjcnasjcnsakcj");

        Assert.Equivalent(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
