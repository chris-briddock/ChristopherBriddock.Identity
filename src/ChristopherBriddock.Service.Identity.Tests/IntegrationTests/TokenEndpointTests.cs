using ChristopherBriddock.Service.Identity.Exceptions;
using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TokenEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private AuthorizeRequest _authorizeRequest;

    public TokenEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _authorizeRequest = default!;
    }

    [Fact]
    public async Task TokenEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        _authorizeRequest = new()
        {
            EmailAddress = "testing@tester.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            RememberMe = true
        };

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "atesty@testing.com" },
                { "Jwt:Expires", "5" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient();

        using var authorizeResponse = await client.PostAsJsonAsync("/authorize", _authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;

        var jsonWebTokenProviderMock = new JsonWebTokenProviderMock();

        jsonWebTokenProviderMock.Setup(s => s.TryCreateRefreshTokenAsync(It.IsAny<string>(),
                                                                         It.IsAny<string>(),
                                                                         It.IsAny<string>(),
                                                                         It.IsAny<string>(),
                                                                         It.IsAny<string>(),
                                                                         It.IsAny<string>())).ThrowsAsync(new CreateJwtException());

        using var tokenClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
            });
        }).CreateClient();

        tokenClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await tokenClient.GetAsync("/token");

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }
}
