namespace Domain.Contracts;

/// <summary>
/// Represents a contract to a soft deletable domain entity.
/// </summary>
public interface ISoftDeletableEntity
{
    /// <summary>
    /// Represents the deleted flag for an auditable entity.
    /// </summary>
    public bool IsDeleted { get; }
    /// <summary>
    /// Represents the delete date and time for an auditable entity.
    /// </summary>
    public DateTime? DeletedOnUtc { get; }
    /// <summary>
    /// Represents who deleted the user.
    /// </summary>
    public Guid? DeletedBy { get; }
}
