using Microsoft.AspNetCore.Mvc;

namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represent a users two factor request to enable or disable the feature.
/// </summary>
public sealed record TwoFactorManageRequest
{
    /// <summary>
    /// Gets or sets the isEnabled flag.
    /// </summary>
    [FromQuery(Name = "is_enabled")]
    public bool IsEnabled { get; init; } = default!;
}
