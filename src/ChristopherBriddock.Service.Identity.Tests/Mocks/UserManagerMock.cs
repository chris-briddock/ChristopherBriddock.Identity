using Castle.Core.Logging;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class UserManagerMock : MockBase<UserManager<ApplicationUser>>
{
    public override UserManager<ApplicationUser> Mock()
    {
        var userManager = Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(), 
                                                                       Substitute.For<IOptions<IdentityOptions>>(),
                                                                       Substitute.For<IPasswordHasher<ApplicationUser>>(),
                                                                       Substitute.For<IEnumerable<IUserValidator<ApplicationUser>>>(),
                                                                       Substitute.For<IEnumerable<IPasswordHasher<ApplicationUser>>>(),
                                                                       Substitute.For<ILookupNormalizer>(),
                                                                       Substitute.For<IServiceProvider>(),
                                                                       Substitute.For<ILogger<UserManager<ApplicationUser>>>());

        return userManager;
    }
}
