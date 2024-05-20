using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Services;
using Microsoft.Extensions.Logging;

namespace ChristopherBriddock.Service.Identity.Tests.IntegrationTests;

public class AccountPurgeBackgroundServiceExposeProtected : AccountPurgeBackgroundService
{
    public AccountPurgeBackgroundServiceExposeProtected(IServiceScopeFactory serviceScopeFactory,
                                                    ILogger<AccountPurgeBackgroundService> logger)
        : base(serviceScopeFactory, logger)
    { }

    // Expose ExecuteAsync
    public Task ExecuteTaskAsync(CancellationToken cancellationToken)
    {
        return base.ExecuteAsync(cancellationToken);
    }
}

[TestFixture]
public class AccountPurgeBackgroundServiceTests
{
    private TestFixture<Program> _fixture;

    public AccountPurgeBackgroundServiceTests()
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
    public async Task ExecuteAsync_DeletesOldUserAccountsAfterSevenYears()
    {
        // Arrange
        var mockLogger = new LoggerMock<AccountPurgeBackgroundService>();
        var client = _fixture.WebApplicationFactory.CreateClient();
        var scopeFactory = _fixture.WebApplicationFactory.Services.GetService<IServiceScopeFactory>()!;

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var oldDeletedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "OldDeletedUser",
            Email = "olddeleted@test.com",
            IsDeleted = true,
            DeletedDateTime = DateTime.Now.AddYears(-8)
        };

        var recentDeletedUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "RecentDeletedUser",
            Email = "recentdeleted@test.com",
            IsDeleted = true,
            DeletedDateTime = DateTime.Now
        };

        dbContext.Users.Add(oldDeletedUser);
        dbContext.Users.Add(recentDeletedUser);

        await dbContext.SaveChangesAsync();

        var service = new AccountPurgeBackgroundServiceExposeProtected(scopeFactory, mockLogger.Object);

        // Act
        await service.ExecuteTaskAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(dbContext.Users.FirstOrDefault(u => u.Id == oldDeletedUser.Id), Is.Null);
            Assert.That(dbContext.Users.Any(u => u.Id == recentDeletedUser.Id), Is.True);
        });
    }

}