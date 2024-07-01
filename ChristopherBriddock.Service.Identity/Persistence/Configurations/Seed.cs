using ChristopherBriddock.Service.Identity.Domain.Aggregates.User;
using Domain.Aggregates.User;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Persistence.Contexts;

namespace Persistence.Configurations;

/// <summary>
/// Provides methods for seeding initial data into the application database.
/// </summary>
public static class Seed
{
    /// <summary>
    /// Seeds roles into the database if they don't already exist.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedRolesAsync(WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextWrite>();

        if (!dbContext.Roles.Any())
        {
            string[] roles = [RoleConstants.Admin, RoleConstants.User];

            // Create roles.
            foreach (var role in roles)
            {
                ApplicationRole newRole = new()
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                };

                if (!await roleManager.RoleExistsAsync(newRole.Name))
                {
                    await roleManager.CreateAsync(newRole);
                }
            }
        }
    }
    /// <summary>
    /// Seeds an admin user into the database if it doesn't already exist.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedAdminUserAsync(WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var adminEmail = "admin@default.com";

        ApplicationUser adminUser = new()
        {
            UserName = adminEmail,
            Email = adminEmail,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            EmailConfirmed = true,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };

        bool userExists = await userManager.FindByEmailAsync(adminEmail) != null;
        if (!userExists)
        {
            // Hash the password for security.
            adminUser.PasswordHash = userManager.PasswordHasher.HashPassword(adminUser, "fR<pGWqvn4Mu,6w[Z8axP;b5=");
            await userManager.CreateAsync(adminUser);

            // Add roles to the admin user.
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
            await userManager.AddToRoleAsync(adminUser, RoleConstants.User);
        }
    }
    /// <summary>
    /// Seeds the initial client application. 
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedApplicationAsync(WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextWrite>();
        var dbSet = dbContext.Set<IdentityApplication>();

        if (!dbSet.Any())
        {
            await dbSet.AddAsync(new IdentityApplication
            {
                Name = "Default",
                RedirectUri = "https://google.com",
                IsDeleted = false,
                CreatedBy = Guid.NewGuid()
            });

            await dbContext.SaveChangesAsync();
        }
    }
    /// <summary>
    /// Seeds all test user data.
    /// </summary>
    public static class Test
    {
        /// <summary>
        /// Seeds an deleted user into the database if it doesn't already exist.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SeedDeletedUser(WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var userEmail = "deletedUser@default.com";

            ApplicationUser user = new()
            {
                UserName = userEmail,
                Email = userEmail,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsDeleted = true,
                DeletedOnUtc = DateTime.UtcNow,
                EmailConfirmed = true,
            };

            // Hash the password for security.
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "fR<pGW'qvn4Mu,6w[Z8axP;b5=");
            await userManager.CreateAsync(user);

            // Add roles to the admin user.
            await userManager.AddToRoleAsync(user, RoleConstants.User);
        }
        /// <summary>
        /// Seeds a user into the database if it doesn't already exist for testing 
        /// the authorize endpoint.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SeedAuthorizeUser(WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var userEmail = "authorizeTest@default.com";

            ApplicationUser user = new()
            {
                UserName = userEmail,
                Email = userEmail,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsDeleted = false,
                DeletedOnUtc = default!,
                EmailConfirmed = true,
            };

            // Hash the password for security.
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "7XAl@Dg()[=8rV;[wD[:GY$yw:$ltHA\\uaf!\\UQ`");
            await userManager.CreateAsync(user);

            // Add roles
            await userManager.AddToRoleAsync(user, RoleConstants.User);
        }

        /// <summary>
        /// Seeds a user into the database if it doesn't already exist for testing 
        /// the authorize endpoint.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SeedTwoFactorUser(WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var userEmail = "twoFactorTest@default.com";

            ApplicationUser user = new()
            {
                UserName = userEmail,
                Email = userEmail,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsDeleted = false,
                DeletedOnUtc = default!,
                EmailConfirmed = true,
            };

            // Hash the password for security.
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "Ar*P`w8R.WyXb7'UKxh;!-");
            await userManager.CreateAsync(user);

            // Add roles
            await userManager.AddToRoleAsync(user, RoleConstants.User);
        }
        /// <summary>
        /// Seeds two test users into the database if it doesn't already exist for testing
        /// 
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SeedBackgroundServiceUsers(WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var oldDeletedUserEmail = "olddeleted@default.com";
            var recentDeletedUserEmail = "recentlydeleted@default.com";

            var oldDeletedUser = new ApplicationUser()
            {
                UserName = oldDeletedUserEmail,
                Email = oldDeletedUserEmail,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsDeleted = true,
                DeletedOnUtc = DateTime.UtcNow.AddYears(-8),
                EmailConfirmed = true

            };

            oldDeletedUser.PasswordHash = userManager.PasswordHasher.HashPassword(oldDeletedUser, "dnjdnjdnwjdnwqjdnqwj");

            var recentDeletedUser = new ApplicationUser
            {
                UserName = recentDeletedUserEmail,
                Email = recentDeletedUserEmail,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsDeleted = true,
                DeletedOnUtc = DateTime.UtcNow,
                EmailConfirmed = true

            };
            recentDeletedUser.PasswordHash = userManager.PasswordHasher.HashPassword(recentDeletedUser, "dnjdnjdnwjdnwqjdnqwj");

            ApplicationUser? oldDeletedUserExists = await userManager.FindByEmailAsync(oldDeletedUserEmail);
            ApplicationUser? recentDeletedUserExists = await userManager.FindByEmailAsync(recentDeletedUserEmail);
            if (oldDeletedUserExists is null
                && recentDeletedUserExists is null)
            {
                await userManager.CreateAsync(oldDeletedUser);
                await userManager.CreateAsync(recentDeletedUser);

                await userManager.AddToRoleAsync(oldDeletedUser, RoleConstants.User);
                await userManager.AddToRoleAsync(recentDeletedUser, RoleConstants.User);
            }

        }
    }

}