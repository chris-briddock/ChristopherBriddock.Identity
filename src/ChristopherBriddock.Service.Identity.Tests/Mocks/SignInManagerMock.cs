using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class SignInManagerMock<T> : IMockBase<Mock<SignInManager<T>>> where T : class
{
    public Mock<SignInManager<T>> Mock()
    {
        return new Mock<SignInManager<T>>(new UserManagerMock<T>().Mock().Object,
                                          new Mock<IHttpContextAccessor>().Object,
                                          new Mock<IUserClaimsPrincipalFactory<T>>().Object,
                                          new Mock<IOptions<IdentityOptions>>().Object,
                                          new Mock<ILogger<SignInManager<T>>>().Object,
                                          new Mock<IAuthenticationSchemeProvider>().Object,
                                          new Mock<IUserConfirmation<T>>().Object);
    }
}
