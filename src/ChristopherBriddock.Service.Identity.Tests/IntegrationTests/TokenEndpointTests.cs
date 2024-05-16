using System.Security.Claims;
using ChristopherBriddock.Service.Identity.Models.Results;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TokenEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public TokenEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientWithMocks(IConfiguration configuration, IJsonWebTokenProvider tokenProvider, ClaimsPrincipal user)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var httpContextMock = new Mock<HttpContext>();

        httpContextMock.Setup(x => x.User).Returns(user);
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
                services.Replace(new ServiceDescriptor(typeof(IConfiguration), configuration));
                services.Replace(new ServiceDescriptor(typeof(IJsonWebTokenProvider), tokenProvider));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions());
    }

    [Fact]
    public async Task TokenEndpoint_Returns200OK_WhenRequestIsValid()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
        configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns("audience");
        configurationMock.SetupGet(x => x["Jwt:Secret"]).Returns("secret");
        configurationMock.SetupGet(x => x["Jwt:Expires"]).Returns("expires");

        var tokenProviderMock = new JsonWebTokenProviderMock();
        tokenProviderMock.Setup(x => x.TryCreateTokenAsync(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>())).ReturnsAsync(new JwtResult { Success = true, Token = "accessToken" });
        tokenProviderMock.Setup(x => x.TryCreateRefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new JwtResult { Success = true, Token = "refreshToken" });
        tokenProviderMock.Setup(x => x.TryValidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new JwtResult { Success = true });

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns("test@test.com");
        claimsPrincipalMock.Setup(x => x.IsInRole("User")).Returns(true);

        var client = CreateClientWithMocks(configurationMock.Object, tokenProviderMock.Object, claimsPrincipalMock.Object);

        // Act
        var sut = await client.GetAsync("/token");

                
        var responseContent = await sut.Content.ReadAsStringAsync();

        // Assert
        sut.EnsureSuccessStatusCode();
        Assert.Equivalent(HttpStatusCode.OK, sut.StatusCode);
    }

    [Fact]
    public async Task TokenEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var tokenProviderMock = new JsonWebTokenProviderMock();
         var configurationMock = new Mock<IConfiguration>();
        configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns("issuer");
        configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns("audience");
        configurationMock.SetupGet(x => x["Jwt:Secret"]).Returns("secret");
        configurationMock.SetupGet(x => x["Jwt:Expires"]).Returns("expires");
        
        tokenProviderMock.Setup(x => x.TryCreateTokenAsync(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var claimsPrincipalMock = new ClaimsPrincipalMock();
        
        claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns("test@test.com");
        claimsPrincipalMock.Setup(x => x.IsInRole("User")).Returns(true);

        var client = CreateClientWithMocks(configurationMock.Object, tokenProviderMock.Object, claimsPrincipalMock.Object);

        // Act
        var response = await client.GetAsync("/token");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
