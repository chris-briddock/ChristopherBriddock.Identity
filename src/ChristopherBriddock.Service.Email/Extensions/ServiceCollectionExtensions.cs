﻿using ChristopherBriddock.Service.Common.Constants;
using MassTransit;
using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Email.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds consumer messages for rabbitmq or azure service bus.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddConsumerMessaging(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider()
                                    .GetService<IConfiguration>()!;

        var featureManager = services.BuildServiceProvider()
                                     .GetService<IFeatureManager>()!;

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.AzServiceBus).Result)
        {
            services.AddMassTransit(mt =>
            {
                mt.SetKebabCaseEndpointNameFormatter();

                mt.AddConsumer<EmailConsumer>();

                mt.UsingAzureServiceBus((context, config) =>
                {
                    config.Host(configuration["Messaging:AzureServiceBus:ConnectionString"]);
                    config.ConfigureEndpoints(context);
                });
            });
        }
        if (featureManager.IsEnabledAsync(FeatureFlagConstants.RabbitMq).Result)
        {

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, config) =>
                {
                    x.SetKebabCaseEndpointNameFormatter();
                    x.AddConsumer<EmailConsumer>();
                    config.Host(configuration["Messaging:RabbitMQ:Hostname"], "/", r =>
                    {
                        r.Username(configuration["Messaging:RabbitMQ:Username"]);
                        r.Password(configuration["Messaging:RabbitMQ:Password"]);
                    });
                    config.ConfigureEndpoints(context);
                });
            });

        }
        return services;
    }
}
