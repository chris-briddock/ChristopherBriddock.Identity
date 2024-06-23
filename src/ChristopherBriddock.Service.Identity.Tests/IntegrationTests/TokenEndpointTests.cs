namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

[TestFixture]
public class TokenEndpointTests
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

    private HttpClient CreateClientWithMocks(IJsonWebTokenProvider tokenProvider, ClaimsPrincipal user)
    {
        var httpContextAccessorMock = new IHttpContextAccessorMock();
        var httpContextMock = new HttpContextMock();

        httpContextMock.Setup(x => x.User).Returns(user);
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        return _fixture.WebApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
                services.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), tokenProvider));

                // Remove the existing authentication scheme
                services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));
                services.RemoveAll(typeof(IPostConfigureOptions<JwtBearerOptions>));

                // Optionally, add a new authentication scheme
                services.AddAuthentication("Identity.Application")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Identity.Application", options => { });
            });
        }).CreateClient(new WebApplicationFactoryClientOptions());
    }

    [Test]
    public async Task TokenEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var tokenProviderMock = new JsonWebTokenProviderMock();

        tokenProviderMock.Setup(x => x.TryCreateTokenAsync(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns("test@test.com");

        var client = CreateClientWithMocks(tokenProviderMock.Object, claimsPrincipalMock.Object);

        // Act
        var sut = await client.GetAsync("/token?token_type=resource_owner");

        // Assert
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task TokenEndpoint_Returns200OK_WithTokenResponse()
    {
        // Arrange
        var tokenProviderMock = new JsonWebTokenProviderMock();

        tokenProviderMock.Setup(p => p.TryCreateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new JwtResult { Success = true, Token = "mockToken", Error = null });

        tokenProviderMock.Setup(p => p.TryCreateRefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new JwtResult { Success = true, Token = "mockRefreshToken", Error = null });

        tokenProviderMock.Setup(p => p.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new JwtResult { Success = true, Token = "mockRefreshToken", Error = null });

        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns("test@test.com");

        var client = CreateClientWithMocks(tokenProviderMock.Object, claimsPrincipalMock.Object);

        // Act
        var response = await client.GetAsync("/token?token_type=resource_owner");

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse!.AccessToken, Is.EqualTo("mockToken"));
            Assert.That(tokenResponse!.RefreshToken, Is.EqualTo("mockRefreshToken"));
            Assert.That(tokenResponse!.Expires, Is.EqualTo("60"));
        });

    }
}