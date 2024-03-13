using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        try
        {
            Logger.LogInformation("Executing {methodName}", nameof(AccountPurgeBackgroundService));

            using var scope = ServiceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<ApplicationUser> usersToBeDeleted =
                        await dbContext.Users
                                    .Where(s =>
                                    s.IsDeleted)
                                    .Where(s =>
                                    s.DeletedDateTime <
                                    DateTime.UtcNow.AddYears(-7))
                                    .ToListAsync(stoppingToken);


            if (!usersToBeDeleted.Any())
            {
                return;
            }
            dbContext.RemoveRange(usersToBeDeleted);
            await Task.Delay(TimeSpan.FromSeconds(120), stoppingToken);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (DbUpdateConcurrencyException ex) 
        {
            Logger.LogWarning("Warning in background service - {methodName}, exception details: {exceptionDetails}", nameof(AccountPurgeBackgroundService), ex);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in background service - {methodName}, exception details: {exceptionDetails}", nameof(AccountPurgeBackgroundService), ex);
        }
    }
}
