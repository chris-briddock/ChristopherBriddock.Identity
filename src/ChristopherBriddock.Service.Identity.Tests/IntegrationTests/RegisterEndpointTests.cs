namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class RegisterEndpointTests
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
    public async Task RegisterEndpoint_Returns201Created_WhenUserIsCreated()
    {
        RegisterRequest registerRequest = new()
        {
            EmailAddress = "abcdefd@test.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            PhoneNumber = "1234567890"
        };

        using var client = _fixture.WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsJsonAsync("/register", registerRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task RegisterEndpoint_Returns409Conflict_WhenUserAlreadyExists()
    {
        RegisterRequest registerRequest = new()
        {
            EmailAddress = "testing@tester.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`",
            PhoneNumber = "1234567890"
        };

        using var client = _fixture.WebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await client.PostAsJsonAsync("/register", registerRequest);

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
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

        using var client = _fixture.WebApplicationFactory.WithWebHostBuilder(s =>
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

        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
}