using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Extensions;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Extensions;
using ChristopherBriddock.Service.Identity.Providers;
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
        builder.Services.AddCustomAuthentication();
        builder.Services.AddCustomAuthorization();
        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddIdentity();
        builder.Services.AddCache();
        builder.Services.AddCustomSession();
        builder.Services.AddResponseCaching();
        builder.Services.AddAzureAppInsights();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddCustomHealthChecks();
        builder.Services.AddCrossOriginPolicy();
        builder.Services.AddPublisherMessaging();
        builder.Services.AddHostedService<AccountPurgeBackgroundService>();
        builder.Services.AddScoped<ILinkProvider, LinkProvider>();

        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(CorsConstants.PolicyName);
        }
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseCustomHealthCheckMapping();
        await app.UseDatabaseMigrationsAsync<AppDbContext>();
        await app.RunAsync();
    }
}