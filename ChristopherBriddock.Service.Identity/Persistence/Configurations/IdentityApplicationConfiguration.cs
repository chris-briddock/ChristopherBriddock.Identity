using ChristopherBriddock.Service.Identity.Domain.Aggregates.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class IdentityApplicationConfiguration : IEntityTypeConfiguration<IdentityApplication>
{
    public void Configure(EntityTypeBuilder<IdentityApplication> builder)
    {
        builder.ToTable("AspNetApplications", opt =>
        {
            opt.IsTemporal();
        });
    }
}
