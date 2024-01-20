using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Extensions;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwagger("ChristopherBriddock.Service.Identity.xml");
builder.Services.AddFeatureManagement();
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentity();
builder.Services.AddCache();
builder.Services.AddCustomSession();
builder.Services.AddResponseCaching();
builder.Services.AddAzureAppInsights();
builder.Services.TryAddScoped<IEmailProvider, EmailProvider>();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddCustomHealthChecks();
builder.Services.AddCrossOriginPolicy();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(CorsConstants.PolicyName);
}
app.UseSession();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCustomHealthCheckMapping();
await app.UseDatabaseMigrationsAsync<AppDbContext>();
await app.RunAsync();