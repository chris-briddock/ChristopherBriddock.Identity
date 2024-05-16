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
    /// <param name="environment"></param>
    /// <returns>The modified <see cref="WebApplication"/> instance.</returns>
    public static async Task<IApplicationBuilder> UseDatabaseMigrationsAsync<TDbContext>(this WebApplication app, IWebHostEnvironment environment) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetService<TDbContext>()!.Database;
        
        // if (!environment.IsEnvironment("Test")) 
        // {
        //     await service.MigrateAsync();
        // }
        
        return app;
    }


}
