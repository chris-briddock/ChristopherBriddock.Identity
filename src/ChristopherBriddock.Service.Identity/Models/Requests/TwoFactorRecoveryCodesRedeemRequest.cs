namespace ChristopherBriddock.Service.Identity.Models.Requests;

public sealed record TwoFactorRecoveryCodesRedeemRequest
{
    public string Code { get; set; }
}
