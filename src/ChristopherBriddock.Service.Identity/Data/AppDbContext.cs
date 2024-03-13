using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChristopherBriddock.Service.Identity.Data;

/// <summary>
/// Represents a session with the database and can be used to query and save instances of your entities.
/// </summary>
/// <param name="configuration"> The application's configuration.</param>
public sealed class AppDbContext(IConfiguration configuration) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    /// <summary>
    /// The application's configuration.
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("Default"),
            opt => opt.EnableRetryOnFailure());

            base.OnConfiguring(optionsBuilder);
    }
}
