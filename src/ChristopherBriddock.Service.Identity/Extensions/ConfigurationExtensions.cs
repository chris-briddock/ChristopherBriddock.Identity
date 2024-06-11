namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Contains extension methods for IConfiguration and IConfigurationBuilder to manage configuration settings.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the specified configuration value or throws an exception if it is not found.
    /// </summary>
    /// <param name="configuration">The configuration instance to extend.</param>
    /// <param name="name">The name of the configuration value.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the configuration value is not found or is empty.</exception>
    public static string GetRequiredValueOrThrow(this IConfiguration configuration, string name)
    {
        var value = configuration[name];
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Value '{name}' not found.");
        }

        return value;
    }
}