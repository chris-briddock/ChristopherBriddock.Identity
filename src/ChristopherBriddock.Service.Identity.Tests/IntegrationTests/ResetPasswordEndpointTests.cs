using Microsoft.AspNetCore.Identity.Data;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class ResetPasswordEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public ResetPasswordEndpointTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task ResetEndpoint_ReturnsStatus204_WhenResetIsSuccessful()
    {
        ResetPasswordRequest resetPasswordRequest = new()
        {
            Email = "test@test.com",
            ResetCode = Convert.ToBase64String(new byte[256]),
            NewPassword = "abcdjenwejfnskdnc"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);
        userManagerMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()));


        using var resetClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = true
        });

        using var sut = await resetClient.PostAsJsonAsync("/resetpassword", resetPasswordRequest);

        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);
    }

    [Fact]
    public async Task ResetEndpoint_ReturnsStatus500_WhenExceptionIsThrown()
    {
        ResetPasswordRequest resetPasswordRequest = new()
        {
            Email = "test@test.com",
            ResetCode = "abcdeefjdk",
            NewPassword = "abcdjenwejfnskdnc"
        };

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.ResetPasswordAsync(It.IsAny<ApplicationUser>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>())).ThrowsAsync(new Exception());


        using var resetClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            });
        }).CreateClient();

        using var sut = await resetClient.PostAsJsonAsync("/resetpassword", resetPasswordRequest);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);
    }
}
