namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class ConfirmEmailEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public ConfirmEmailEndpointTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task ConfirmEmailEndpoint_ReturnsStatus500_WithInvalidRequest()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(s => s.ConfirmEmailAsync(It.IsAny<ApplicationUser>(),
                                                       It.IsAny<string>())).ThrowsAsync(new Exception());

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        ConfirmEmailRequest confirmEmailRequest = new()
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        using var sut = await client.PostAsync($"/confirmemail?EmailAddress=test%40test.com&Code={confirmEmailRequest.Code}", null);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmailEndpoint_ReturnsStatus_200OKWithValidRequest()
    {
        ConfirmEmailRequest confirmEmailRequest = new()
        {
            EmailAddress = "test@test.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(s => s.ConfirmEmailAsync(It.IsAny<ApplicationUser>(),
                                                       It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        using var sut = await client.PostAsync($"/confirmemail?EmailAddress=test%40test.com&Code={confirmEmailRequest.Code}", null);

        Assert.Equivalent(HttpStatusCode.OK, sut.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmailEndpoint_ReturnsStatus_500WhenUserIsNotFound()
    {
        ConfirmEmailRequest confirmEmailRequest = new()
        {
            EmailAddress = "test@asdf.com",
            Code = "CfDJ8OgQt3GRCbpAqpW/lUkYbKcxoL55kAWWuaMIq6/+FPUL4p7KYF6W5u89C2yjXp/NANvDtxLbOggkSvJs24z/cM7PW1iDmiegeS4f9XLHLBQlVzQWKaYZou4rIWKTBxk9O4sFFTC7006koe3sUS0URACV4Iq0Xw3EON2hm+3ji05UgFz+JHLZ7Oou7063fEBmmfDjpbTP9Lk5YobeYEddf6rCkSLC786AYkht+xM0x0g7"
        };

        using var client = _webApplicationFactory.WithWebHostBuilder(s =>
        {
        }).CreateClient();

        using var sut = await client.PostAsync($"/confirmemail?EmailAddress={confirmEmailRequest.EmailAddress}&Code={confirmEmailRequest.Code}", null);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }


}
