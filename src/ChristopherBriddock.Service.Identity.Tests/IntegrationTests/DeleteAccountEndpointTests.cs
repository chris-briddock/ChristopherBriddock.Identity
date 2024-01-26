using ChristopherBriddock.Service.Identity.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using AuthorizeRequest = ChristopherBriddock.Service.Identity.Models.Requests.AuthorizeRequest;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;


public class DeleteAccountEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public DeleteAccountEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
    }

    [Fact]
    public async Task DeleteAccountEndpoint_Returns204_WhenAccountIsDeleted()
    {
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "atesty@testing.com",
            Password = "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHAauaf!aUQ`",
            RememberMe = true
        };

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Issuer", "https://localhost" },
                { "Jwt:Secret", "=W0Jqcxsz8] Lq74z*:&gB^zmhx*HsrB6GYj%K}G" },
                { "Jwt:Audience", "atesty@testing.com" },
                { "Jwt:Expires", "5" }
            }).Build();

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {

                s.Replace(new ServiceDescriptor(typeof(IConfiguration), configurationBuilder));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var authorizeResponse = await client.PostAsJsonAsync("/authorise", authorizeRequest);

        var jsonDocumentRoot = JsonDocument.Parse(authorizeResponse.Content.ReadAsStream()).RootElement;

        string? accessToken = jsonDocumentRoot.GetProperty("accessToken").GetString()!;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var sut = await client.DeleteAsync("/account/delete");

        Assert.Equivalent(HttpStatusCode.OK, authorizeResponse.StatusCode);
        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);
    }
}
