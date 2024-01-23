using ChristopherBriddock.Service.Identity.Tests.Mocks;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class ConfirmEmailEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private ConfirmEmailRequest _confirmEmailRequest;

    public ConfirmEmailEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
    { 
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _confirmEmailRequest = default!;
    }

    [Fact]
    public async Task ConfirmEmailEndpoint_ReturnsStatus500_WithInvalidRequest()
    {
        using var client = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        _confirmEmailRequest = new()
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        using var sut = await client.PostAsync($"/confirmemail?EmailAddress=test%40test.com&Code=abdef", null);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmailEndpoint_ReturnsStatus_200OKWithValidRequest()
    {
        _confirmEmailRequest = new()
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        UserManagerMock userManagerMock = new();
        var mock = userManagerMock.Mock().When(x => x.ConfirmEmailAsync(Arg.Any<ApplicationUser>(),
                                                                        Arg.Any<string>())
                                                     .Returns(IdentityResult.Success));

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), mock));
            });
        }).CreateClient();

        using var sut = await client.PostAsync($"/confirmemail?EmailAddress=test%40test.com&Code=abdef", null);

        Assert.Equivalent(HttpStatusCode.OK, sut.StatusCode);
    }

    //[Fact]
    //public async Task ConfirmEmailEndpoint_ReturnsStatus_404WhenUserIsNotFound()
    //{
    //    _confirmEmailRequest = new()
    //    {
    //        EmailAddress = "test@test.com",
    //        Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
    //    };




    //}
}
