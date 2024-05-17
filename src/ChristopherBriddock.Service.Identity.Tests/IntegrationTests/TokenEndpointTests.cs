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
    public async Task TokenEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var tokenProviderMock = new JsonWebTokenProviderMock();
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "audience" },
                { "Jwt:Expires", "60" },
            }).Build();
        
        tokenProviderMock.Setup(x => x.TryCreateTokenAsync(It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>(),
                                                           It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var claimsPrincipalMock = new ClaimsPrincipalMock();
        
        claimsPrincipalMock.Setup(x => x.Identity!.Name).Returns("test@test.com");

        var client = CreateClientWithMocks(configurationBuilder, tokenProviderMock.Object, claimsPrincipalMock.Object);

        // Act
        var sut = await client.GetAsync("/token");

        var response = await sut.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }
}
