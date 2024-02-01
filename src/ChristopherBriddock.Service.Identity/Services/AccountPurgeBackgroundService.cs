using ChristopherBriddock.Service.Identity.Data;
using Microsoft.AspNetCore.SignalR;

namespace ChristopherBriddock.Service.Identity.Services;

/// <summary>
/// This backround service deletes old user account marked as deleted after seven years.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="AccountPurgeBackgroundService"/>
/// </remarks>
/// <param name="serviceScopeFactory">A factory for creating instances of <see cref="IServiceScope"/></param>
/// <param name="logger">The application logger.</param>
public class AccountPurgeBackgroundService(IServiceScopeFactory serviceScopeFactory,
                                           ILogger<AccountPurgeBackgroundService> logger) : BackgroundService
{
    /// <summary>
    /// A factory for creating instances of <see cref="IServiceScope"/>
    /// </summary>
    public IServiceScopeFactory ServiceScopeFactory { get; } = serviceScopeFactory;

    /// <summary>
    /// The application logger.
    /// </summary>
    public ILogger<AccountPurgeBackgroundService> Logger { get; } = logger;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

        try
        {
            Logger.LogInformation("Executing {methodName}", nameof(AccountPurgeBackgroundService));

            using var scope = ServiceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userToBeDeleted = dbContext.Users.Where(s => s.IsDeleted)
                                .Where(s => s.DeletedDateTime < DateTime.Today.AddYears(-7));

            dbContext.RemoveRange(userToBeDeleted);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in background service - {methodName}, exception details: {exceptionDetails}", nameof(AccountPurgeBackgroundService), ex);
        }
    }
}
