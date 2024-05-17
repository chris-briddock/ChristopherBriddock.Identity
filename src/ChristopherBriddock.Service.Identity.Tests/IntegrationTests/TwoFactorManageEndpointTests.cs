﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class TwoFactorManageEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public TwoFactorManageEndpointTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task TwoFactorManageEndpoint_Returns204NoContent_WhenTwoFactorIsEnabled()
    {

        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        userManagerMock.Setup(s => s.GetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false);

        userManagerMock.Setup(s => s.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>())).ReturnsAsync(IdentityResult.Success);

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

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
            });
        }).CreateClient();

        using var sut = await sutClient.PostAsync("/2fa/manage?IsEnabled=true", null);

        Assert.Equivalent(HttpStatusCode.NoContent, sut.StatusCode);

    }

    [Fact]
    public async Task TwoFactorManageEndpoint_Returns400BadRequest_WhenTwoFactorFailsToEnable()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.GetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false);
        userManagerMock.Setup(s => s.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>())).ReturnsAsync(IdentityResult.Failed());

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
        
        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
                s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
                s.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
            });
        }).CreateClient();

        using var sut = await sutClient.PostAsync("/2fa/manage?IsEnabled=true", null);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }

    [Fact]
    public async Task TwoFactorManageEndpoint_Returns500InternalServerError_WhenAnExceptionIsThrown()
    {
        var userManagerMock = new UserManagerMock<ApplicationUser>().Mock();

        userManagerMock.Setup(s => s.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
        userManagerMock.Setup(s => s.GetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false);
        userManagerMock.Setup(s => s.SetTwoFactorEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>())).ThrowsAsync(new Exception());

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

        using var sutClient = _webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.ConfigureTestServices(s =>
            {
               s.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), userManagerMock.Object));
               s.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), httpContextAccessorMock.Object));
            });
        }).CreateClient();

        using var sut = await sutClient.PostAsync("/2fa/manage?IsEnabled=true", null);

        Assert.Equivalent(HttpStatusCode.InternalServerError, sut.StatusCode);

    }
}
