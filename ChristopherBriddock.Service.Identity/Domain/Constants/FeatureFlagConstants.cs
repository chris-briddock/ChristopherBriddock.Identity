using Microsoft.FeatureManagement;

namespace Domain.Constants;

/// <summary>
/// Contains configuration definitions for features that can be disabled or enabled.
/// </summary>
public static class FeatureFlagConstants
{
    /// <summary>
    /// Gets or sets the boolean to enable or disable Redis caching.
    /// </summary>
    /// <remarks>
    /// This const string value is used by <see cref="Microsoft.FeatureManagement"/> to check 
    /// the appsetings.json when this method is called <see cref="IFeatureManager.IsEnabledAsync"/>
    /// </remarks>
    public const string Redis = "Redis";
    /// <summary>
    /// Gets or sets the boolean to enable or disable Azure Application Insights.
    /// </summary>
    /// <remarks>
    /// This const string value is used by <see cref="Microsoft.FeatureManagement"/> to check 
    /// the appsetings.json when this method is called <see cref="IFeatureManager.IsEnabledAsync"/>
    /// </remarks>
    public const string AzApplicationInsights = "ApplicationInsights";
    /// <summary>
    /// Gets or sets the boolean to enable or disable the external logging server.
    /// </summary>
    /// <remarks>
    /// This const string value is used by <see cref="Microsoft.FeatureManagement"/> to check 
    /// the appsetings.json when this method is called <see cref="IFeatureManager.IsEnabledAsync"/>
    /// </remarks>
    public const string Seq = "Seq";
    /// <summary>
    /// Gets or sets the boolean to enable or disable 
    /// </summary>
    /// <remarks>
    /// This const string value is used by <see cref="Microsoft.FeatureManagement"/> to check 
    /// the appsetings.json when this method is called <see cref="IFeatureManager.IsEnabledAsync"/>
    /// </remarks>
    public const string AzServiceBus = "ServiceBus";
    /// <summary>
    /// Gets or sets the boolean to enable or disable 
    /// </summary>
    /// <remarks>
    /// This const string value is used by <see cref="Microsoft.FeatureManagement"/> to check 
    /// the appsetings.json when this method is called <see cref="FeatureManager.IsEnabledAsync"/>
    /// </remarks>
    public const string RabbitMq = "RabbitMq";


}
