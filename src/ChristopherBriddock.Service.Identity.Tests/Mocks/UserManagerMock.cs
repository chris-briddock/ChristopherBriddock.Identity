using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChristopherBriddock.Service.Identity.Tests.Mocks;

internal class UserManagerMock<T> : IMockBase<Mock<UserManager<T>>> where T : class
{
    public Mock<UserManager<T>> Mock()
    {
        var userManagerMock = new Mock<UserManager<T>>(
            new Mock<IUserStore<T>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<T>>().Object,
            new IUserValidator<T>[0],
            new IPasswordValidator<T>[0],
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<T>>>().Object);

        return userManagerMock;
    }
}
