namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Extensions for <see cref="IApplicationBuilder"/>
/// </summary>
public static class WebApplicationExtensions
{

    /// <summary>
    /// Seeds data asynchronously.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task SeedDataAsync(this WebApplication app)
    {
        await Seed.SeedRolesAsync(app);
        await Seed.SeedAdminUserAsync(app);
        await Seed.SeedApplicationAsync(app);
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
