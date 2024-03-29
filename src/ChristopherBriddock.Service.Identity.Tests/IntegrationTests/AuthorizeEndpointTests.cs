﻿namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class AuthorizeEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private AuthorizeRequest _authorizeRequest;

    public AuthorizeEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _authorizeRequest = default!;
    }
    [Fact]
    public async Task AuthorizeEndpoint_ReturnsStatus302Found_WhenValidCredentialsAreUsed()
    {
        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        _authorizeRequest = new()
        {
            EmailAddress = "testing@tester.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            RememberMe = true
        };

        using var sut = await client.PostAsJsonAsync("/authorize", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Found, sut.StatusCode);
    }
    /// <summary>
    /// Test
    /// </summary>
    [Fact]
    public async Task AuthorizeEndpoint_ReturnsStatus401Unauthorized_WhenUseInvalidValidCredentials()
    {
        using var client = _webApplicationFactory.CreateClient();

        _authorizeRequest = new()
        {
            EmailAddress = "test@abc.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        using var sut = await client.PostAsJsonAsync("/authorize", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }
    /// <summary>
    /// Test
    /// </summary>
    [Fact]
    public async Task AuthorizeEndpoint_ReturnsStatus500InternalServerError_WhenExceptionIsThrown()
    {
        var signInManagerMock = new SignInManagerMock<ApplicationUser>().Mock();

        signInManagerMock.Setup(s => s.PasswordSignInAsync(It.IsAny<ApplicationUser>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<bool>(),
                                                           It.IsAny<bool>())).ThrowsAsync(new Exception());


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
                s.Replace(new ServiceDescriptor(typeof(SignInManager<ApplicationUser>), signInManagerMock.Object));
            });
        }).CreateClient();

        using var sut = await client.PostAsJsonAsync("/authorize", _authorizeRequest);
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }

    [Fact]
    public async Task AuthorizeEndpoint_ReturnsStatus302Found_WhenTwoFactorIsEnabled()
    {
        _authorizeRequest = new()
        {
            EmailAddress = "test@asdf.com",
            Password = "Ar*P`w8R.WyXb7'UKxh;!-",
            RememberMe = true
        };

        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });


        using var sut = await client.PostAsJsonAsync("/authorize", _authorizeRequest);
        Assert.Equivalent(HttpStatusCode.Found, sut.StatusCode);

    }

    [Fact]
    public async Task AuthorizeEndpoint_Returns401Unauthorized_WhenUserIsDeleted()
    {
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "test@euiop.com",
            Password = "w?M`YBqR6}*X,87):$u+eQ",
            RememberMe = true
        };
        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        using var sut = await client.PostAsJsonAsync("/authorize", authorizeRequest);
        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }
}

