using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.AspNetCore.HealthChecks;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Extensions;
using ChristopherBriddock.Service.Identity.Services;
using Microsoft.FeatureManagement;

namespace ChristopherBriddock.Service.Identity;
/// <summary>
/// The entry point for the Web Application.
/// </summary>
public sealed class Program
{

    private Program() { }

    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwagger("ChristopherBriddock.Service.Identity.xml");
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
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddSqlDatabaseHealthChecks(builder.Configuration.GetConnectionStringOrThrow("Default"));
        builder.Services.AddAzureApplicationInsightsHealthChecks();
        builder.Services.AddSeqHealthCheckPublisher();
        builder.Services.AddRedisHealthChecks(builder.Configuration["ConnectionStrings:Redis"]!);
        builder.Services.AddCorsPolicy();
        builder.Services.AddPublisherMessaging();
        builder.Services.AddHostedService<AccountPurgeBackgroundService>();
        builder.Services.AddAppRateLimiting();

        WebApplication app = builder.Build();
        app.UseHsts();
        app.UseResponseCaching();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCustomHealthCheckMapping();
        await app.UseDatabaseMigrationsAsync<AppDbContext>();
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