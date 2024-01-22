using ChristopherBriddock.Service.Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Serilog;

namespace ChristopherBriddock.Service.Common.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds serilog to console by default, optionally adds Seq by setting the feature flag.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddSerilogWithConfiguration(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider()
                                   .GetRequiredService<IConfiguration>();

        var featureManager = services.BuildServiceProvider()
                             .GetRequiredService<IFeatureManager>();

        services.AddSerilog();

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.Seq).Result)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom
                                                  .Configuration(configuration)
                                                  .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration().WriteTo
                                                  .Console()
                                                  .CreateLogger();
        }

        services.AddLogging(loggingbuilder =>
        {
            loggingbuilder.AddSerilog(Log.Logger);
        });

        return services;
    }
}
