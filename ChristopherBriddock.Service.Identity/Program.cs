
using ChristopherBriddock.AspNetCore.Extensions;
using Microsoft.FeatureManagement;
using Application.Extensions;
using Persistence.Contexts;
using Application.BackgroundServices;
using ChristopherBriddock.AspNetCore.HealthChecks;
using Domain.Constants;

namespace ChristopherBriddock.Service.Identity;

/// <summary>
/// The entry point for the Web Application.
/// </summary>
public sealed class Program
{
    /// <summary>
    /// Initializes a new instance of <see cref="Program"/>
    /// </summary>
    private Program() { }

    ///
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwagger("ChristopherBriddock.Service.Identity.xml");
        builder.Services.AddVersioning(1, 0);
        builder.Services.AddDatabaseServices();
        builder.Services.AddFeatureManagement();
        builder.Services.AddSerilogWithConfiguration();
        builder.Services.AddBearerAuthentication();
        builder.Services.AddAuthorizationPolicy();
        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddIdentity();
        builder.Services.AddAppSession();
        builder.Services.AddSessionCache();
        builder.Services.AddResponseCaching();
        builder.Services.AddAzureAppInsights();
        builder.Services.AddSqlDatabaseHealthChecks(builder.Configuration.GetConnectionStringOrThrow("Default"));
        builder.Services.AddAzureApplicationInsightsHealthChecks();
        builder.Services.AddSeqHealthCheckPublisher();
        builder.Services.AddRedisHealthChecks(builder.Configuration["ConnectionStrings:Redis"]!);
        builder.Services.AddCorsPolicy();
        builder.Services.AddPublisherMessaging();
        builder.Services.AddHostedService<AccountPurgeBackgroundService>();
        builder.Services.AddAppRateLimiting();


        var app = builder.Build();
        app.UseHsts();
        app.UseResponseCaching();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCustomHealthCheckMapping();
        await app.UseDatabaseMigrationsAsync<AppDbContextWrite>();
        await app.SeedDataAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(CorsConstants.PolicyName);
            await app.SeedTestDataAsync();
        }
        await app.RunAsync();
    }
}
