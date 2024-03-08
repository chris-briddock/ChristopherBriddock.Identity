using System.Security.Cryptography;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Provides methods to generate secure random codes.
/// </summary>
public sealed class CodeProvider : ICodeProvider
{
    /// <inheritdoc/>
    public string Create(int length = 4)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}