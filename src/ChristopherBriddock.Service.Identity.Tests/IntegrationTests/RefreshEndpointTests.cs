using ChristopherBriddock.Service.Identity.Models.Results;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class RefreshEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public RefreshEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task RefreshEndpoint_Returns401_WhenInvalidTokenIsUsed()
    {
        using var client = _webApplicationFactory.CreateClient();

        var accessToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhdGVzdHlAdGVzdGluZy5jb20iLCJqdGkiOiIzNzM1ZTQwOS04MzIzLTRiN2MtYjczZC00ZWZkNWRmMmRjZWUiLCJpYXQiOiIxNzA2NDA4MjQ0IiwiZW1haWwiOiJhdGVzdHlAdGVzdGluZy5jb20iLCJleHAiOjE3MDY2MjQyNDQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0IiwiYXVkIjoiYXRlc3R5QHRlc3RpbmcuY29tIn0.YeOVALkBLkdxPgHg5EpV54tuQYgwwGPC - VitAe1Wubc";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await client.PostAsync("/refresh", null);

        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }

    [Fact]
    public async Task RefreshEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
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
                { "Jwt:Audience", "atesty@testing.com" },
                { "Jwt:Expires", "60" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {

                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var authorizeResponse = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;
        string refreshToken = jsonDocumentRoot.GetProperty("refreshToken").GetString()!;

        JsonWebTokenProviderMock jsonWebTokenProviderMock = new();

        RefreshRequest refreshRequest = new() { RefreshToken = refreshToken };

        jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>())).ThrowsAsync(new Exception());

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient();

        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var sut = await sutClient.PostAsJsonAsync("/refresh", refreshRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }

    [Fact]
    public async Task RefreshEndpoint_Returns302Found_WhenRefreshIsSuccessful()
    {

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "atesty@testing.com" },
                { "Jwt:Expires", "5" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {

                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "authenticationtest@test.com",
            Password = "Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*",
            RememberMe = true
        };

        using var authorizeResponse = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;
        string? refreshToken = jsonDocumentRoot.GetProperty("refreshToken").GetString()!;

        RefreshRequest refreshRequest = new() { RefreshToken = refreshToken };


        using var refreshClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {

                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        refreshClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var sut = await refreshClient.PostAsJsonAsync("/refresh", refreshRequest);

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);

        Assert.Equivalent(HttpStatusCode.Found, sut.StatusCode);
    }
}
