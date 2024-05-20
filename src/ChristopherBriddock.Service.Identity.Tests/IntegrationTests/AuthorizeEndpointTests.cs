namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class AuthorizeEndpointTests
{
    private TestFixture<Program> _fixture;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _fixture = new TestFixture<Program>();
        _fixture.OneTimeSetUp();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _fixture.OneTimeTearDown();
        _fixture.Dispose();
        _fixture = null!;
    }

    [Test]
    public async Task AuthorizeEndpoint_ReturnsStatus302Found_WhenValidCredentialsAreUsed()
    {
        // Arrange
        using var client = _fixture.WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "testing@tester.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            RememberMe = true
        };

        // Act
        using var response = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Found));
    }

    [Test]
    public async Task AuthorizeEndpoint_ReturnsStatus401Unauthorized_WhenUseInvalidValidCredentials()
    {
        // Arrange
        using var client = _fixture.WebApplicationFactory.CreateClient();

        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "test@abc.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        // Act
        using var response = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task AuthorizeEndpoint_ReturnsStatus500InternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var signInManagerMock = new SignInManagerMock<ApplicationUser>().Mock();

        signInManagerMock.Setup(s => s.PasswordSignInAsync(
            It.IsAny<ApplicationUser>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>())).ThrowsAsync(new Exception());

        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "test@test.com",
            Password = "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G",
            RememberMe = true
        };

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(SignInManager<ApplicationUser>), signInManagerMock.Object));
            });
        }).CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task AuthorizeEndpoint_ReturnsStatus200OK_WhenTwoFactorIsEnabled()
    {
        // Arrange
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "test@asdf.com",
            Password = "Ar*P`w8R.WyXb7'UKxh;!-",
            RememberMe = true
        };

        using var client = _fixture.WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        using var response = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task AuthorizeEndpoint_ReturnsStatus401Unauthorized_WhenUserIsDeleted()
    {
        // Arrange
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "test@euiop.com",
            Password = "w?M`YBqR6}*X,87):$u+eQ",
            RememberMe = true
        };

        using var client = _fixture.WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        using var response = await client.PostAsJsonAsync("/authorize", authorizeRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}