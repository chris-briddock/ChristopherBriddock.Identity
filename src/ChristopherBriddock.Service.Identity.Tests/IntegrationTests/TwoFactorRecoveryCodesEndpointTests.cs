namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorRecoveryCodesEndpointTests
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
    public async Task TwoFactorRecoveryCodesEndpoint_Returns200OK_WhenRecoveryCodesAreRequested()
    {
        // Arrange

        // Create a new instance of ApplicationUser with desired properties
        var user = new ApplicationUser();

        // Create a mock instance of UserManager<ApplicationUser>
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        // Set up mock behavior for FindByEmailAsync and GenerateNewTwoFactorRecoveryCodesAsync methods
        userManagerMock.Setup(s => s.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        userManagerMock.Setup(s => s.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ReturnsAsync(new[] { "code1", "code2" });

        // Create a mock instance of ClaimsPrincipal and set up behavior for FindFirst method
        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(u => u.FindFirst(ClaimTypes.Email))
            .Returns(new Claim(ClaimTypes.Email, "test@test.com"));

        // Create a mock instance of HttpContext and set up the User property
        var httpContextMock = new HttpContextMock();
        httpContextMock.Setup(x => x.User).Returns(claimsPrincipalMock.Object);

        // Create a mock instance of IHttpContextAccessor
        var httpContextAccessorMock = new IHttpContextAccessorMock();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        // Create an instance of HttpClient with the necessary services configured
        var client = _fixture.CreateAuthenticatedClient(x => 
        {
            x.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            x.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
        }); 
                
        // Act
        var sut = await client.GetAsync("/2fa/codes");
        // Assert
        sut.EnsureSuccessStatusCode();
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task TwoFactorRecoveryCodesEndpoint_Returns500InternalServerError_WhenExceptionIsThrown()
    {
        // Create a new instance of ApplicationUser with desired properties
        var user = new ApplicationUser();

        // Create a mock instance of UserManager<ApplicationUser>
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        // Create a mock instance of ClaimsPrincipal and set up behavior for FindFirst method
        var claimsPrincipalMock = new ClaimsPrincipalMock();
        claimsPrincipalMock.Setup(u => u.FindFirst(ClaimTypes.Email))
            .Throws(new Exception());

        // Create a mock instance of HttpContext and set up the User property
        var httpContextMock = new HttpContextMock();
        httpContextMock.Setup(x => x.User).Returns(claimsPrincipalMock.Object);

        // Create a mock instance of IHttpContextAccessor
        var httpContextAccessorMock = new IHttpContextAccessorMock();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        var sutClient = _fixture.CreateAuthenticatedClient(x => 
        {
            x.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
            x.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
        });

        using var sut = await sutClient.GetAsync("/2fa/codes");
        Assert.That(sut.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

    }
}