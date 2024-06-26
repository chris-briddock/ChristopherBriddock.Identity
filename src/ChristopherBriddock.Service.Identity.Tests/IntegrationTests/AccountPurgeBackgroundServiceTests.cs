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
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var service = new AccountPurgeBackgroundServiceExposeProtected(scopeFactory, mockLogger.Object);

        var oldDeletedUser = dbContext.Users
        .Where(s => s.Email == "deletedUser@default.com")
        .Where(x => x.DeletedOnUtc <= DateTime.UtcNow.AddYears(-8))
        .FirstOrDefault()!;

        var recentDeletedUser = dbContext.Users.Where(s => s.Email == "recentlydeleted@default.com").First();

        // Act
        await service.ExecuteTaskAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(dbContext.Users.FirstOrDefault(u => u == oldDeletedUser), Is.Null);
            Assert.That(dbContext.Users.Any(u => u.Id == recentDeletedUser.Id), Is.True);
        });
    }

}