using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.Service.Identity.Domain.Aggregates.User;
using Domain.Aggregates.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Persistence.Contexts;

/// <summary>
/// Represents a session with the database and can be used to query and save instances of your entities.
/// </summary>
public sealed class AppDbContextWrite : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    /// <summary> 
    /// An instance of the application configuration.
    /// </summary>
    public IConfiguration Configuration { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContextWrite"/> class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    /// <param name="configuration">The application configuration settings.</param>
    public AppDbContextWrite(DbContextOptions options,
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    /// <summary>
    /// Represents a database table with the columns defined in the properties of <see cref="IdentityApplication"/>
    /// </summary>
    public DbSet<IdentityApplication> AspNetApplications => Set<IdentityApplication>();
}
