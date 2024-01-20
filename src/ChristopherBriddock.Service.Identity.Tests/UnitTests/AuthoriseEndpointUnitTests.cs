using ChristopherBriddock.Service.Identity.Endpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ChristopherBriddock.Service.Identity.Tests.UnitTests;

public class AuthoriseEndpointUnitTests
{
    [Fact]
    public async void AuthoriseEndpointReturns302Found_WhenLocalRedirectToTokenIsCalled()
    {

        // Arrange
        AuthorizeRequest authorizeRequest = new()
        {
            EmailAddress = "email@email.com",
            Password = "password",
            RememberMe = true,
        };

        IServiceProvider serviceProviderMock = new ServiceProviderMock().Mock();
        ILogger<AuthoriseEndpoint> loggerMock = new LoggerMock<AuthoriseEndpoint>().Mock();
        SignInManager<ApplicationUser> signInManagerMock = new SignInManagerMock().Mock();

        serviceProviderMock.GetRequiredService<SignInManager<ApplicationUser>>().Returns(signInManagerMock);
        signInManagerMock.PasswordSignInAsync(authorizeRequest.EmailAddress,
                                                    authorizeRequest.Password,
                                                    authorizeRequest.RememberMe,
                                                    false).Returns(Arg.Any<Microsoft.AspNetCore.Identity.SignInResult>());

        var controller = new AuthoriseEndpoint(serviceProviderMock, loggerMock);

        // Act
        var result = await controller.HandleAsync(authorizeRequest);

        // Assert

        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status302Found, ((StatusCodeResult)result).StatusCode);

    }
}