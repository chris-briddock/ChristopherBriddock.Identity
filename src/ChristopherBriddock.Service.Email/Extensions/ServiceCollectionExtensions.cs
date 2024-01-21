using ChristopherBriddock.Service.Common.Constants;
using MassTransit;
using Microsoft.FeatureManagement;
using Serilog;
using System.Reflection;

namespace ChristopherBriddock.Service.Email.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumerMessaging(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider()
                                    .GetRequiredService<IConfiguration>();

        var featureManager = services.BuildServiceProvider()
                                     .GetRequiredService<IFeatureManager>();
        services.AddMassTransit(x =>
        {

            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumers(typeof(Program).Assembly);

            if (featureManager.IsEnabledAsync(FeatureFlagConstants.AzServiceBus).Result)
            {
                services.AddMassTransit(mt =>
                {
                    mt.UsingAzureServiceBus((context, config) =>
                    {
                        config.Host(configuration["Messaging:AzureServiceBus:ConnectionString"]);
                        config.ConfigureEndpoints(context);
                    });
                });
            }
            if (featureManager.IsEnabledAsync(FeatureFlagConstants.RabbitMq).Result)
            {
                x.UsingRabbitMq((context, config) =>
                {
                    x.AddConsumer<EmailConsumer>();
                    config.Host(configuration["Messaging:RabbitMQ:Hostname"], "/", r =>
                    {
                        r.Username(configuration["Messaging:RabbitMQ:Username"]);
                        r.Password(configuration["Messaging:RabbitMQ:Password"]);
                    });
                    config.ConfigureEndpoints(context);
                });
            }
        });
        
        return services;
    }

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
