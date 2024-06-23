namespace ChristopherBriddock.Service.Identity.Models.Entities;

/// <summary>
/// Defines a contract for auditable entities.
/// </summary>
public interface IAuditableEntity 
{
    /// <summary>
    /// Represents the deleted flag for an auditable entity.
    /// </summary>
    public bool IsDeleted { get; set; }
    /// <summary>
    /// Represents the delete date and time for an auditable entity.
    /// </summary>
    public DateTime? DeletedOnUtc { get; set; }
    /// <summary>
    /// Represents who deleted the user.
    /// </summary>
    public Guid? DeletedBy { get; set; } 
    /// <summary>
    /// Represents the created date and time for an auditable entity.
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
    /// <summary>
    /// Represents who modified the user.
    /// </summary>
    public Guid? CreatedBy { get; set; }
    /// <summary>
    /// Represents the modified date and time for an auditable entity.
    /// </summary>
    public DateTime? ModifiedOnUtc { get; set; }
    /// <summary>
    /// Represents who modified the user.
    /// </summary>
    public Guid? ModifiedBy { get; set; }


}