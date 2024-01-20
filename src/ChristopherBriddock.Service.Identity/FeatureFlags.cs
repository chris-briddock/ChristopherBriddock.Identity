namespace ChristopherBriddock.Service.Identity;

/// <summary>
/// Contains configuration definitions for features that can be disabled or enabled.
/// </summary>
public static class FeatureFlags
{
    /// <summary>
    /// Gets or sets the boolean to enable or disable Redis caching.
    /// </summary>
    public const string Redis = "Redis";
    /// <summary>
    /// Gets or sets the boolean to enable or disable Azure Application Insights.
    /// </summary>
    public const string ApplicationInsights = "ApplicationInsights";
    /// <summary>
    /// Gets or sets the boolean to enable or disable the external logging server.
    /// </summary>
    public const string ExternalLoggingServer = "ExternalLoggingServer";
}
