namespace ChristopherBriddock.Service.Identity.Models.Requests;

/// <summary>
/// Represents a two factor sign in request.
/// </summary>
public sealed record TwoFactorSignInRequest
{
    /// <summary>
    /// The two factor token
    /// </summary>
    public string Token { get; set; } = default!;
}
