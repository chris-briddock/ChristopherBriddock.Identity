using ChristopherBriddock.AspNetCore.Extensions;
using Domain.Constants;
using Microsoft.FeatureManagement;

namespace Application.Extensions;

/// <summary>
/// Health check related extension methods.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Adds health check publishing to Seq.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to health checks will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddSeqHealthCheckPublisher(this IServiceCollection services) 
    {
        var featureManager = services.BuildServiceProvider()
                                     .GetRequiredService<IFeatureManager>();
        
        var configuration = services.BuildServiceProvider()
                                    .GetRequiredService<IConfiguration>();
        
        if (featureManager.IsEnabledAsync(FeatureFlagConstants.Seq).Result)
        {
            services.AddHealthChecks().AddSeqPublisher(opt =>
            {
                opt.Endpoint = configuration.GetRequiredValueOrThrow("Seq:Endpoint")!;
                opt.ApiKey = configuration.GetRequiredValueOrThrow("Seq:ApiKey");
            });
        }

        return services;

    }
    /// <summary>
    /// Adds health checks for vital infrastructure such as Databases, Caches and Logging.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to health checks will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAzureApplicationInsightsHealthChecks(this IServiceCollection services)
    {
        var featureManager = services.BuildServiceProvider()
                                     .GetRequiredService<IFeatureManager>();

        var configuration = services.BuildServiceProvider()
                                    .GetRequiredService<IConfiguration>();

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.AzApplicationInsights).Result)
        {
            var key = configuration.GetRequiredValueOrThrow("ApplicationInsights:InstrumentationKey");
            services.AddHealthChecks().AddAzureApplicationInsights(key);
        }

        return services;
    }
}
