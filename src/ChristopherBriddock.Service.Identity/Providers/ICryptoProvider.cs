namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// Defines a contract for providing methods to generate and verify cryptographically hashed strings.
/// </summary>

public interface ICryptoProvider 
{
    /// <summary>
    /// Generates a cryptographically hashed string using PBKDF2 with HMAC-SHA512.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A string containing the salt and hashed password separated by a colon.</returns>
    public string GenerateHash(string password);

    /// <summary>
    /// Verifies a password by comparing it with a stored hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="storedHash">The stored hash of the password (including the salt).</param>
    /// <returns>True if the provided password matches the stored hash, otherwise false.</returns>
    public bool VerifyHash(string password, string storedHash);
}