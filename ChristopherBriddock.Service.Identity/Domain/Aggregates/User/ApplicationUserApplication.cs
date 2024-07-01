using Domain.Aggregates.User;

namespace ChristopherBriddock.Service.Identity.Domain.Aggregates.User;

/// <summary>
/// Represents the link between user and application.
/// </summary>
public sealed class ApplicationUserApplication
{
    public Guid ApplicationId { get; set; }
    public IdentityApplication IdentityApplication { get; set; } = default!;

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = default!;
}
