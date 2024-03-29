﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristopherBriddock.Service.Identity.Models;

/// <inheritdoc/>
public sealed class ApplicationRole : IdentityRole<Guid>
{
    /// <inheritdoc/>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override Guid Id { get; set; } = Guid.NewGuid();
}
