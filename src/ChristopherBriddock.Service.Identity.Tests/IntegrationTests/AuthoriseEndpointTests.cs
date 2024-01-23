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
    public async Task AuthoriseEndpoint_ReturnsStatus302Found_WhenValidCredentialsAreUsed()
    {
        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        _authorizeRequest = new()
        {
            EmailAddress = "test@test.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        var sut = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Found, sut.StatusCode);
    }
    /// <summary>
    /// Test
    /// </summary>
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

        using var sut = await client.PostAsJsonAsync("/authorise", _authorizeRequest);

        Assert.Equivalent(HttpStatusCode.Unauthorized, sut.StatusCode);
    }
    /// <summary>
    /// Test
    /// </summary>
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

        using var sut = await client.PostAsJsonAsync("/authorise", _authorizeRequest);
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);


    }
}
