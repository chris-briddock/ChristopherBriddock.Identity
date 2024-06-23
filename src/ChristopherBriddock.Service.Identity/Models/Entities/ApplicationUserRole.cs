using Microsoft.AspNetCore.Identity;

namespace ChristopherBriddock.Service.Identity.Models.Entities;

/// <inheritdoc/>
public sealed class ApplicationUserRole : IdentityUserRole<Guid>, IAuditableEntity
{
}
