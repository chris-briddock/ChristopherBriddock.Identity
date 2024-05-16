using ChristopherBriddock.Service.Identity.Models.Results;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class RefreshEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public RefreshEndpointTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
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
        JsonWebTokenProviderMock jsonWebTokenProviderMock = new();

        RefreshRequest refreshRequest = new() { RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs" };

        jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>())).ThrowsAsync(new Exception());

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
            });
        }).CreateClient();

        using var sut = await sutClient.PostAsJsonAsync("/refresh", refreshRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }

    [Fact]
    public async Task RefreshEndpoint_Returns302Found_WhenRefreshIsSuccessful()
    {
        var jsonWebTokenProviderMock = new JsonWebTokenProviderMock();
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "atesty@testing.com" },
                { "Jwt:Expires", "5" }
            }).Build();

        RefreshRequest refreshRequest = new() { RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNocmlzIiwic3ViIjoiY2hyaXMiLCJqdGkiOiJmYTAxNGFmZSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjI0NTMzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzODMiLCJodHRwczovL2xvY2FsaG9zdDo3MDc4IiwiaHR0cDovL2xvY2FsaG9zdDo1MTc0Il0sIm5iZiI6MTcxNTg3MjQ2OCwiZXhwIjoxNzIzODIxMjY4LCJpYXQiOjE3MTU4NzI0NjksImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.-xKTBzDGb1dDMDsYWn_W3ARhK0MEon_6aO63LBs_2Xs" };

        jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>())).ReturnsAsync(new JwtResult(){
                                                                        Success = true
                                                                    });


        using var refreshClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        using var sut = await refreshClient.PostAsJsonAsync("/refresh", refreshRequest);
        

        var responseContent = await sut.Content.ReadAsStringAsync();

        Assert.Equivalent(HttpStatusCode.Found, sut.StatusCode);
    }

    [Fact]
    public async Task RefreshEndpoint_Returns401Unauthorized_WhenTokenValidationFails()
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

        var jsonWebTokenProviderMock = new JsonWebTokenProviderMock().Mock();

        JwtResult jwtResult = new()
        {
            Success = false
        };

        jsonWebTokenProviderMock.Setup(s => s.TryValidateTokenAsync(It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>(),
                                                                    It.IsAny<string>())).ReturnsAsync(jwtResult);

        using var refreshClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), jsonWebTokenProviderMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        refreshClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await refreshClient.PostAsJsonAsync("/refresh", refreshRequest);

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }
}
