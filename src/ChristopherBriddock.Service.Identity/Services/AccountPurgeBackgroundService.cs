using ChristopherBriddock.Service.Identity.Data;

namespace ChristopherBriddock.Service.Identity.Services;

/// <summary>
/// This backround service deletes old user account marked as deleted after seven years.
/// </summary>
public class AccountPurgeBackgroundService : BackgroundService
{
    /// <summary>
    /// The service scope factory.
    /// </summary>
    public IServiceScopeFactory ServiceScopeFactory { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AccountPurgeBackgroundService"/>
    /// </summary>
    /// <param name="serviceScopeFactory">A factory for creating instances of <see cref="IServiceScope"/></param>
    public AccountPurgeBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

        using var scope = ServiceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var userToBeDeleted = dbContext.Users.Where(s => s.IsDeleted)
                            .Where(s => s.DeletedDateTime < DateTime.Today.AddYears(-7));


        dbContext.RemoveRange(userToBeDeleted);
        await dbContext.SaveChangesAsync(stoppingToken);

    }
}
