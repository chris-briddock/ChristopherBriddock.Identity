using ChristopherBriddock.Service.Identity.Endpoints;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Net.Http.Json;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class AuthoriseEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private AuthorizeRequest _authorizeRequest;

    public AuthoriseEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _authorizeRequest = default!;
    }

    [Fact]
    public async Task AuthoriseEndpoint_ReturnsStatus302Found_WhenUseValidCredentials()
    {
        using var client = _webApplicationFactory.CreateClient();

        _authorizeRequest = new()
        {
            EmailAddress = "test@test.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        var endpointResponse = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Redirect, endpointResponse.StatusCode);
    }
    [Fact]
    public async Task AuthoriseEndpoint_ReturnsStatus401Unauthorized_WhenUseInvalidValidCredentials()
    {
        using var client = _webApplicationFactory.CreateClient();

        _authorizeRequest = new()
        {
            EmailAddress = "test@abc.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        using var endpointResponse = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Unauthorized, endpointResponse.StatusCode);
    }
    [Fact]
    public async Task AuthoriseEndpoint_ReturnsStatus500InternalServerError_WhenExceptionIsThrown()
    {
        LoggerMock<AuthoriseEndpoint> loggerMock = new();

        loggerMock.Mock().Throws<Exception>();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(services =>
            {
                services.AddSingleton<LoggerMock<AuthoriseEndpoint>>();
            });
        }).CreateClient();

        using var endpointResponse = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, endpointResponse.StatusCode);
    }
}
