namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class RegisterEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{

    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public RegisterEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task RegisterEndpoint_Returns201Created_WhenUserIsCreated()
    {
        RegisterRequest registerRequest = new()
        {
            EmailAddress = "abcdef@test.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            PhoneNumber = "1234567890"
        };

        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsJsonAsync("/register", registerRequest);

        Assert.Equivalent(HttpStatusCode.Created, sut.StatusCode);
    }

    [Fact]
    public async Task RegisterEndpoint_Returns409Conflict_WhenUserAlreadyExists()
    {
        RegisterRequest registerRequest = new()
        {
            EmailAddress = "testing@tester.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            PhoneNumber = "1234567890"
        };

        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsJsonAsync("/register", registerRequest);

        Assert.Equivalent(HttpStatusCode.Conflict, sut.StatusCode);
    }

    [Fact]
    public async Task RegisterEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        RegisterRequest registerRequest = new()
        {
            EmailAddress = "test@euiop.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            PhoneNumber = "1234567890"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.SetUserNameAsync(It.IsAny<ApplicationUser>(),
                                                      It.IsAny<string>())).ThrowsAsync(new Exception());

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });


        using var sut = await client.PostAsJsonAsync("/register", registerRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }
}
