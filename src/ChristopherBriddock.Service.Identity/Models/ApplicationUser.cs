using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristopherBriddock.Service.Identity.Models;

/// <inheritdoc/>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <inheritdoc/>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// Gets or sets whether the user is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
