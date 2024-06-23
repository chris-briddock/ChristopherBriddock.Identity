namespace ChristopherBriddock.Service.Identity.Constants;

/// <summary>
/// Constant values for different token types.
/// </summary>
public static class TokenConstants 
{
    /// <summary>
    /// Represents the value for resource owner token type.
    /// </summary>
    public const string ResourceOwner = "resource_owner";
    /// <summary>
    /// Represents the value for client credentials token type.
    /// </summary>
    public const string ClientCredentials = "client_credentials";
    /// <summary>
    /// Represents the value for device code token type.
    /// </summary>
    public const string DeviceCode = "device_code";
}