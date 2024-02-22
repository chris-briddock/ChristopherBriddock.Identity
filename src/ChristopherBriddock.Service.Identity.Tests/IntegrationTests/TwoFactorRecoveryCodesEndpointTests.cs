using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorRecoveryCodesEndpointTests : IClassFixture<WebApplicationFactory<Program>> 
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public TwoFactorRecoveryCodesEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task TwoFactorRecoveryCodesEndpoint_Returns200OK_WhenRecoveryCodesAreRequested() 
    {
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "authenticationtest@test.com",
            Password = "Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*",
            RememberMe = true
        };

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "authenticationtest@test.com" },
                { "Jwt:Expires", "60" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });

        using var authorizeResponse = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        userManagerMock.Setup(s => s.GenerateNewTwoFactorRecoveryCodesAsync(user, 10));

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient();

        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await sutClient.GetAsync("/2fa/codes");

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.OK, sut.StatusCode);
    }

     [Fact]
    public async Task TwoFactorRecoveryCodesEndpoint_Returns500InternalServerError_WhenExceptionIsThrown() {
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "authenticationtest@test.com",
            Password = "Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*",
            RememberMe = true
        };

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "authenticationtest@test.com" },
                { "Jwt:Expires", "60" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });

        using var authorizeResponse = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        var user = new ApplicationUser();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient();

        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await sutClient.GetAsync("/2fa/codes");

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }
}