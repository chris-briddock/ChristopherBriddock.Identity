using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class SignInManagerMock : MockBase<SignInManager<ApplicationUser>>
{
    public override SignInManager<ApplicationUser> Mock()
    {
        var signInManager = Substitute.For<SignInManager<ApplicationUser>>(
            Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(),
                                                                       Substitute.For<IOptions<IdentityOptions>>(),
                                                                       Substitute.For<IPasswordHasher<ApplicationUser>>(),
                                                                       Substitute.For<IEnumerable<IUserValidator<ApplicationUser>>>(),
                                                                       Substitute.For<IEnumerable<IPasswordHasher<ApplicationUser>>>(),
                                                                       Substitute.For<ILookupNormalizer>(),
                                                                       Substitute.For<IServiceProvider>(),
                                                                       Substitute.For<ILogger<UserManager<ApplicationUser>>>()),
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<ILogger<SignInManager<ILogger>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<ApplicationUser>>()
            );

        return signInManager;
    }
}
