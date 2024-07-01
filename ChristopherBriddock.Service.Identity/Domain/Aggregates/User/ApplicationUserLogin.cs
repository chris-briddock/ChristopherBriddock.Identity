using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.User;

/// <inheritdoc/>
public sealed class ApplicationUserLogin : IdentityUserLogin<Guid>, IAuditableEntity, ISoftDeletable
{
    /// <inheritdoc/>
    public bool IsDeleted { get; set; } = false;
    /// <inheritdoc/>
    public DateTime? DeletedOnUtc { get; set; } = null!;
    /// <inheritdoc/>
    public Guid? DeletedBy { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    /// <inheritdoc/>
    public Guid? CreatedBy { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime? ModifiedOnUtc { get; set; } = null!;
    /// <inheritdoc/>
    public Guid? ModifiedBy { get; set; } = null!;
}
