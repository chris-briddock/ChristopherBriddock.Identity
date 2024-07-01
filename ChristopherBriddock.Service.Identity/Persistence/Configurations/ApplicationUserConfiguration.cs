using Domain.Aggregates.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// Configuration class for the ApplicationUser entity.
/// This class is responsible for configuring the database schema for ApplicationUser.
/// </summary>
/// <remarks>
/// This class implements IEntityTypeConfiguration to provide entity configuration for Entity Framework Core.
/// It is sealed to prevent inheritance, as it's designed to be a specific configuration for <see cref="ApplicationUser"/>
/// </remarks>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="ApplicationUser"/>
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    /// <remarks>
    /// This method is called by Entity Framework Core to configure the ApplicationUser entity.
    /// It sets up the Address property as a complex type.
    /// </remarks>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers", opt =>
        {
            opt.IsTemporal();
        });
        builder.ComplexProperty(x => x.Address);
    }
}
