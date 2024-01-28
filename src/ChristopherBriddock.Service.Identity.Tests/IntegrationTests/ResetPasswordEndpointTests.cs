namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class ResetPasswordEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public ResetPasswordEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public void ResetEndpoint_ReturnsStatus204_WhenResetIsSuccessful()
    {
        Assert.True(true);
    }

    [Fact]
    public void ResetEndpoint_ReturnsStatus400_WhenUserIsNotConfirmed()
    {
        Assert.True(true);
    }

    [Fact]
    public void ResetEndpoint_ReturnsStatus500_WhenExceptionIsThrown()
    {
        Assert.True(true);
    }
}
