using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
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
    public async Task AuthoriseEndpoint_ReturnsStatus200OK_WhenRedirectedToToken()
    {
        using var client = _webApplicationFactory.CreateClient();

        RegisterRequest registerRequest = new()
        {
            EmailAddress = "test@test.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            PhoneNumber = "180055501000"
        };
        _authorizeRequest = new()
        {
            EmailAddress = registerRequest.EmailAddress,
            Password = registerRequest.Password,
            RememberMe = true
        };

        var endpointResponse = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.OK, endpointResponse.StatusCode);
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
        ServiceProviderMock serviceProviderMock = new();
        var mock = serviceProviderMock.Mock()
                                      .When(x => x.GetRequiredService<SignInManager<ApplicationUser>>()
                                      .Throws(new InvalidOperationException()));

        _authorizeRequest = new()
        {
            EmailAddress = "test@test.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(SignInManager<ApplicationUser>), mock));
            });
        }).CreateClient();

        using var endpointResponse = await client.PostAsJsonAsync("/authorise", _authorizeRequest);
        Assert.Equivalent(HttpStatusCode.InternalServerError, endpointResponse.StatusCode);


    }
}
