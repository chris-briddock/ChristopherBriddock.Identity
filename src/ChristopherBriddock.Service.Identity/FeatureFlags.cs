namespace ChristopherBriddock.Service.Identity;

/// <summary>
/// Contains configuration definitions for features that can be disabled or enabled.
/// </summary>
public static class FeatureFlags
{
    /// <summary>
    /// Gets or sets the boolean to enable or disable Redis caching.
    /// </summary>
    public static string Redis { get; set; } = "Redis";
    /// <summary>
    /// Gets or sets the boolean to enable or disable Azure Application Insights.
    /// </summary>
    public static string ApplicationInsights { get; set; } = "ApplicationInsights";
    /// <summary>
    /// Gets or sets the boolean to enable or disable the external logging server.
    /// </summary>
    public static string ExternalLoggingServer { get; set; } = "ExternalLoggingServer";
}
