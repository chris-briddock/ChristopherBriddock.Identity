using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristopherBriddock.Service.Identity.Models.Entities;

/// <summary>
/// Represents an application in the identity system. 
/// </summary>
public class IdentityApplication : IdentityApplication<Guid>, IAuditableEntity
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


/// <summary>
/// Base class for ASP.NET applications representing OAuth 2.0 clients.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier key.</typeparam>
public class IdentityApplication<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier key for the application.
    /// </summary>
    public virtual TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the globally unique identifier for the application.
    /// </summary>
    public virtual Guid ClientId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the client secret used for authentication.
    /// </summary>
    public virtual string ClientSecret { get; set; } = Ulid.NewUlid().ToString();

    /// <summary>
    /// Gets or sets the name of the client application.
    /// </summary>
    public virtual string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the redirect URI for the client application.
    /// This URI is used by the authorization server to 
    /// redirect the user after granting authorization.
    /// </summary>
    public virtual string RedirectUri { get; set; } = default!;
    /// <summary>
    /// Gets or sets the state value.
    /// This is to be passed as a query parameter to the authorize endpoint.
    /// </summary>
    public virtual string State { get; set; } = Ulid.NewUlid().ToString();
}
