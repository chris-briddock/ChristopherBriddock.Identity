using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.User;

/// <inheritdoc/>
public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
}
