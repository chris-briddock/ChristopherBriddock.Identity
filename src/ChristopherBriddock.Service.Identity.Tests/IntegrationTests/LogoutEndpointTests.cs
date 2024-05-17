using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class LogoutEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public LogoutEndpointTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task LogoutEndpoint_Returns401Unauthorized_WhenNotLoggedIn()
    {
        using var client = _webApplicationFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        using var sut = await client.PostAsync("/logout", null);

        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }

    [Fact]
    public async Task LogoutEndpoint_Returns204NoContent_WhenUserLogsOutSuccessfully()
    {
        var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsync("/logout", null);
        
        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);
    }

    [Fact]
    public async Task LogoutEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        var signInManagerMock = new SignInManagerMock<ApplicationUser>().Mock();

        signInManagerMock.Setup(s => s.SignOutAsync()).ThrowsAsync(new Exception());

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
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

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }
}
