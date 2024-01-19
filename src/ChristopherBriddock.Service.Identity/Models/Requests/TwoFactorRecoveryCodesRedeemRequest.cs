namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a two factor recovery code redeem request.
/// </summary>
public sealed record TwoFactorRecoveryCodesRedeemRequest
{
    /// <summary>
    /// The two factor recovery code.
    /// </summary>
    public string Code { get; set; } = default!;
}
