using Microsoft.EntityFrameworkCore;

namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Extensions for <see cref="IApplicationBuilder"/>
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Automatically applies pending database migrations.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="app"></param>
    /// <returns>The modified <see cref="WebApplication"/> instance.</returns>
    public static async Task<IApplicationBuilder> UseDatabaseMigrationsAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TDbContext>().Database;
        await service.MigrateAsync();
        return app;
    }


}
