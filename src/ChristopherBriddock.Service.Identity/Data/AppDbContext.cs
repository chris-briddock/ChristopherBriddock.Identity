using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChristopherBriddock.Service.Identity.Data;

/// <summary>
/// Represents a session with the database and can be used to query and save instances of your entities.
/// </summary>
public sealed class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    /// <summary> 
    /// An instance of the application configuration.
    /// </summary>
    public IConfiguration Configuration { get; }
    /// <summary>
    /// Initializes a new instance of <see cref="AppDbContext"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public AppDbContext(DbContextOptions options,
                        IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionStringOrThrow("Default"), opt => 
        {
            opt.EnableRetryOnFailure();
        });
        base.OnConfiguring(optionsBuilder);
    }
}
