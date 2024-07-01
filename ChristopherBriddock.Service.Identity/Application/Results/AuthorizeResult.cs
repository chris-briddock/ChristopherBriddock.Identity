using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Application.Results;

/// <summary>
/// Represents the result of an authorization attempt.
/// </summary>
public class AuthorizeResult : ResultBase<AuthorizeResult>
{
    private static readonly AuthorizeResult _twoFactorRequired = new() { TwoFactorRequired = true };

    /// <summary>
    /// Gets a result indicating that two-factor authentication is required.
    /// </summary>
    public static AuthorizeResult RequiresTwoFactor => _twoFactorRequired;

    public bool TwoFactorRequired { get; private set; }
}

