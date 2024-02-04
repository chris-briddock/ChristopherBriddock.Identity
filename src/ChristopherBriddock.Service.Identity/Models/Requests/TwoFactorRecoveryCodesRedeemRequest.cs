namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a two factor recovery code redeem request.
/// </summary>
public sealed record TwoFactorRecoveryCodesRedeemRequest
{
    /// <summary>
    /// Gets or sets the user email address which the recovery code is used for.
    /// </summary>
    public required string EmailAddress { get; set; } = default!;
    /// <summary>
    /// Gets or sets the recovery code.
    /// </summary>
    public required string Code { get; set; } = default!;
}
