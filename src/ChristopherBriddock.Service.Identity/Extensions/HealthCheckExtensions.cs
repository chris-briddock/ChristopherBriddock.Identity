using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using System.Text;
using System.Text.Json;

namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Health check related extension methods.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Maps a custom health check endpoint to the specified route.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to which the health check mapping is added.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> for further configuration.</returns>
    public static IEndpointRouteBuilder UseCustomHealthCheckMapping(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResultStatusCodes =
       {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
       },
            ResponseWriter = HealthCheckResponseWriter.WriteResponse,
            AllowCachingResponses = true
        });

        return app;
    }

    /// <summary>
    /// Adds health checks for vital infrastructure such as Databases, Caches and Logging.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which authentication services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        var featureManager = services.BuildServiceProvider()
                                     .GetRequiredService<IFeatureManager>();

        var configuration = services.BuildServiceProvider()
                                    .GetRequiredService<IConfiguration>();

        services.AddHealthChecks()
                .AddNpgSql(configuration.GetConnectionString("Default")!);


        if (featureManager.IsEnabledAsync(FeatureFlags.Redis).Result)
        {
            services.AddHealthChecks().AddRedis(configuration["ConnectionStrings:Redis"]!,
                                                null,
                                                HealthStatus.Unhealthy,
                                                null,
                                                null);
        }
        
        if (featureManager.IsEnabledAsync(FeatureFlags.ExternalLoggingServer).Result)
        {
            services.AddHealthChecks().AddSeqPublisher(opt =>
            {
                opt.Endpoint = configuration["Seq:Endpoint"]!;
                opt.ApiKey = configuration["Seq:ApiKey"]!;
            });
        }

        return services;
    }
}


internal static class HealthCheckResponseWriter
{
    internal static async Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        await context.Response.WriteAsync(
           Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}