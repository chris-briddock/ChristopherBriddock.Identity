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

public class AccountPurgeBackgroundServiceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly Mock<ILogger<AccountPurgeBackgroundService>> _mockLogger;

    public AccountPurgeBackgroundServiceTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(s =>
        {
            s.UseEnvironment("Test");
        });
        _mockLogger = new Mock<ILogger<AccountPurgeBackgroundService>>();
    }

    [Fact]
    public async Task ExecuteAsync_DeletesOldUserAccountsAfterSevenYears()
    {
        // Arrange
        var client = _webApplicationFactory.CreateClient();

        var scopeFactory = _webApplicationFactory.Services.GetService<IServiceScopeFactory>()!;

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        dbContext.Users.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "User1",
            Email = "user1@test.com",
            IsDeleted = true,
            DeletedDateTime = DateTime.Now.AddYears(-8)
        });

        dbContext.Users.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "User2",
            Email = "user2@test.com",
            IsDeleted = true,
            DeletedDateTime = DateTime.Now
        });
        await dbContext.SaveChangesAsync();

        var service = new AccountPurgeBackgroundServiceExposeProtected(scopeFactory, _mockLogger.Object);

        // Act
        await service.ExecuteTaskAsync(CancellationToken.None);

        // Assert
        Assert.DoesNotContain(dbContext.Users, u => u.IsDeleted && u.DeletedDateTime < DateTime.Today.AddYears(-7));
    }
}