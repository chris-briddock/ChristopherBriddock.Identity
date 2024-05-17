using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChristopherBriddock.Service.Identity.Data;

/// <summary>
/// Represents a session with the database and can be used to query and save instances of your entities.
/// </summary>
/// <param name="configuration"> The application's configuration.</param>
/// <param name="webHostEnvironment">Provides information about the web host environment.</param>
public sealed class AppDbContext(IConfiguration configuration,
                                 IWebHostEnvironment webHostEnvironment) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    /// <summary>
    /// The application's configuration.
    /// </summary>
    private IConfiguration Configuration { get; } = configuration;
    /// <summary>
    /// The web host environment provides information about the environment it is running in.
    /// </summary>
    public IWebHostEnvironment WebHostEnvironment { get; } = webHostEnvironment;
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (WebHostEnvironment.IsEnvironment("Development"))
        {
            optionsBuilder.UseSqlite("Data Source=LocalDatabase.db");
        }
        else
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("Default"),
            opt => opt.EnableRetryOnFailure());
        }

        base.OnConfiguring(optionsBuilder);
    }
}
