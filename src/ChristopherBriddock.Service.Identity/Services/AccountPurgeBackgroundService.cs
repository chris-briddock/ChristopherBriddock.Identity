using ChristopherBriddock.Service.Identity.Data;

namespace ChristopherBriddock.Service.Identity.Services;

/// <summary>
/// This backround service deletes old user account marked as deleted after seven years.
/// </summary>
/// <param name="appDbContext">The application's database context</param>
public class AccountPurgeBackgroundService(AppDbContext appDbContext) : BackgroundService
{
    /// <summary>
    /// The application's database context.
    /// </summary>
    public AppDbContext AppDbContext { get; } = appDbContext;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

        var userToBeDeleted = AppDbContext.Users.Where(s => s.IsDeleted)
                            .Where(s => s.DeletedDateTime < DateTime.Today.AddYears(-7));


        AppDbContext.RemoveRange(userToBeDeleted);
        await AppDbContext.SaveChangesAsync(stoppingToken);

    }
}
