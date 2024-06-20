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
    public static async Task<IApplicationBuilder> UseDatabaseMigrationsAsync<TDbContext>(this IApplicationBuilder app, IWebHostEnvironment environment) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TDbContext>().Database;

        // temp debug
        var temp = service.GetConnectionString();
        
        await service.MigrateAsync();

        return app;
    }

    /// <summary>
    /// Seeds data asynchronously.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task SeedDataAsync(this WebApplication app)
    {
        await Seed.SeedRolesAsync(app);
        await Seed.SeedAdminUserAsync(app);
    }
    /// <summary>
    /// Seeds all test data asynchronously
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task SeedTestDataAsync(this WebApplication app)
    {
        await Seed.Test.SeedAuthorizeUser(app);
        await Seed.Test.SeedDeletedUser(app);
        await Seed.Test.SeedTwoFactorUser(app);
        await Seed.Test.SeedBackgroundServiceUsers(app);
    }
}
