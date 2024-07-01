using Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Aggregates.User;

/// <inheritdoc/>
public sealed class ApplicationRole : IdentityRole<Guid>, IAuditableEntity, ISoftDeletable
{
    /// <inheritdoc/>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override Guid Id { get; set; } = Guid.NewGuid();
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
