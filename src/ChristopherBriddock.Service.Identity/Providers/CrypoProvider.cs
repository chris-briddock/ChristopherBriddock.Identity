using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ChristopherBriddock.Service.Identity.Providers;

/// <summary>
/// A utility class for cryptographic operations.
/// </summary>
public sealed class CryptoProvider : ICryptoProvider
{
    /// <summary>
    /// Generates a random salt.
    /// </summary>
    /// <returns>A byte array containing a random salt.</returns>
    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltSizes.Size512 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
    /// <inheritdoc/>
    public string GenerateHash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        byte[] salt = GenerateSalt();
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 250000,
            numBytesRequested: SaltSizes.Size512 / 8);

        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verifies a password by comparing it with a stored hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="storedHash">The stored hash of the password (including the salt).</param>
    /// <returns>True if the provided password matches the stored hash, otherwise false.</returns>
    public bool VerifyHash(string password, string storedHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(storedHash);
        
        string[] parts = storedHash.Split(':');
        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        byte[] enteredPasswordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 250000,
            numBytesRequested: SaltSizes.Size512 / 8);

        return enteredPasswordHash.SequenceEqual(hash);
    }
}

/// <summary>
/// A class that defines salt sizes for cryptographic operations.
/// </summary>
public static class SaltSizes
{
    /// <summary>
    /// Represents a salt size of 512 bits.
    /// </summary>
    public const int Size512 = 512;

    /// <summary>
    /// Represents a salt size of 256 bits.
    /// </summary>
    public const int Size256 = 256;

    /// <summary>
    /// Represents a salt size of 128 bits.
    /// </summary>
    public const int Size128 = 128;
}