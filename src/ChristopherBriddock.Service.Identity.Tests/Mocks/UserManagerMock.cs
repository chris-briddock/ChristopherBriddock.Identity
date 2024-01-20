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
        return Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(),
                                                                       Substitute.For<IOptions<IdentityOptions>>(),
                                                                       Substitute.For<IPasswordHasher<ApplicationUser>>(),
                                                                       Substitute.For<IEnumerable<IUserValidator<ApplicationUser>>>(),
                                                                       Substitute.For<IEnumerable<IPasswordValidator<ApplicationUser>>>(),
                                                                       Substitute.For<ILookupNormalizer>(),
                                                                       Substitute.For<IdentityErrorDescriber>(),
                                                                       Substitute.For<IServiceProvider>(),
                                                                       Substitute.For<ILogger<UserManager<ApplicationUser>>>());

    }
}
