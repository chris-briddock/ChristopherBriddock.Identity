﻿using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class LogoutEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public LogoutEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task LogoutEndpoint_Returns401Unauthorized_WhenNotLoggedIn()
    {
        using var client = _webApplicationFactory.CreateClient();

        using var sut = await client.PostAsync("/logout", null);

        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }

    [Fact]
    public async Task LogoutEndpoint_Returns204NoContent_WhenUserLogsOutSuccessfully()
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

        var client = _webApplicationFactory.WithWebHostBuilder(s =>
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

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await client.PostAsync("/logout", null);

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);
    }

    [Fact]
    public async Task LogoutEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
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

        var signInManagerMock = new SignInManagerMock<ApplicationUser>().Mock();

        signInManagerMock.Setup(s => s.SignOutAsync()).ThrowsAsync(new Exception());

        using var clientWithForcedError = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(SignInManager<ApplicationUser>), signInManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        clientWithForcedError.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await clientWithForcedError.PostAsync("/logout", null);

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }
}
