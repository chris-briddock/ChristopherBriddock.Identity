using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.Service.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
    /// An instance of the memory cache interface.
    /// </summary>
    public IMemoryCache MemoryCache { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    /// <param name="configuration">The application configuration settings.</param>
    /// <param name="cache">The in-memoru cache.</param>
    public AppDbContext(DbContextOptions options,
                        IConfiguration configuration,
                        IMemoryCache cache) : base(options)
    {
        Configuration = configuration;
        MemoryCache = cache;
    }
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionStringOrThrow("Default"), opt => 
        {
            opt.EnableRetryOnFailure();
            opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
        optionsBuilder.UseMemoryCache(MemoryCache);
        
        base.OnConfiguring(optionsBuilder);
    }
}
