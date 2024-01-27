using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Identity.Publishers;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class ForgotPasswordEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private ForgotPasswordRequest _forgotPasswordRequest;
    public ForgotPasswordEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _forgotPasswordRequest = default!;
    }
    [Fact]
    public async Task ForgotPasswordEndpoint_Returns204NoContent_WhenRequestIsValid()
    {

        _forgotPasswordRequest = new()
        {
            EmailAddress = "test@euiop.com"
        };

        var nullEmailPublisherMock = new Mock<NullEmailPublisher>();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.RemoveAllKeyed(typeof(IEmailPublisher), KeyedServiceNameConstants.EmailProviderNullImplementation);
                s.AddTransient<IEmailPublisher, NullEmailPublisher>();
            });
        }).CreateClient();

        using var sut = await client.PostAsJsonAsync("/forgotpassword", _forgotPasswordRequest);

        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);

    }
    [Fact]
    public async Task ForgotPasswordEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        _forgotPasswordRequest = new()
        {
            EmailAddress = "test@asdf.com"
        };



        using var client = _webApplicationFactory.WithWebHostBuilder(s => s.ConfigureTestServices(_ =>
        {
            _.RemoveAll<IEmailPublisher>();
        })).CreateClient();

        using var sut = await client.PostAsJsonAsync("/forgotpassword", _forgotPasswordRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }


}
